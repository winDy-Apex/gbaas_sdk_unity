using System;
using System.Collections.Generic;
using GBaaS.io.Objects;
using GBaaS.io.Utils;
using System.Threading;

namespace GBaaS.io.Services
{
	class GBUserService : GBService<GBUserService>
	{
		private string _userNmae = "";
		private string _userUUID = "";

		public GBUserService() {}

		public List<GBaaSApiHandler> _handler = new List<GBaaSApiHandler>();

		public void SetHandler(GBaaSApiHandler handler) {
			if (handler == null) {
				_handler.Clear();
			} else {
				_handler.Add(handler);
			}
		}

		private bool IsAsync(bool forceSync = false) {
			return (_handler.Count > 0 && forceSync == false);
		}

		public string GetLoginName() {
			return _userNmae;
		}

		public string GetUserUUID() {
			return _userUUID;
		}

		public void SetUserUUID(string uuid) {
			_userUUID = uuid;
		}

		public GBResult Login(string userName, string password, bool forceSync = false) {
			if (IsAsync(forceSync)) {
				(new Thread(() => this.LoginThread(userName, password))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.LoginThread(userName, password, forceSync);
			}
		}

		private GBResult LoginThread(string userName, string password, bool forceSync = false) {
			string accessToken = "";
			GBResult result = new GBResult();

			try {
				accessToken = GBRequestService.Instance.GetToken(userName, password);
				if (accessToken.Length > 0) {
					HttpHelper.Instance._accessToken = accessToken;
					_userNmae = userName;
					result.MakeResult(true, ReturnCode.SuccessWithReasonAsResult, accessToken);
				} else {
					result.MakeResult(false, ReturnCode.FailWithReason, "Access Token is Null");
				}
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());
			}

			if (IsAsync(forceSync)) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnLogin(result);
				}
			}

			return result;
		}

		public GBResult LoginWithFaceBook(string facebookToken) {
			if (IsAsync()) {
				(new Thread(() => this.LoginWithFaceBookThread(facebookToken))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.LoginWithFaceBookThread(facebookToken);
			}
		}

		private GBResult LoginWithFaceBookThread(string facebookToken) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/auth/facebook?fb_access_token=" + facebookToken);

			GBResult result = new GBResult();

			if (rawResults.Length > 0) {
				string accessToken = GBRequestService.Instance.GetValueFromJson("access_token", rawResults);

				if (accessToken.Length > 0) {
					HttpHelper.Instance._accessToken = accessToken;
					result.MakeResult(true, ReturnCode.Success, accessToken);
				} else {
					result.MakeResult(false, ReturnCode.FailWithReason, "Access Token is Null");
				}

				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnLoginWithFaceBook(result);
					}
				} else {
					return result;
				}
			} else {
				result.MakeResult(false, ReturnCode.FailWithReason, "Raw Result is Null");

				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnLoginWithFaceBook(result);
					}
				} else {
					return result;
				}
			}

			return result;
		}

		// GBaaS 에 CreateUser 과정을 거치지 않고
		// Device 에서 획득한 deviceID 또는 사용자 어플리케이션 단위로 생성한 UUID 등을
		// 암묵적인 사용자 ID로 사용하여 로그인 한다.
		// 이 경우 UpdateUserName 을 호출하여 사용자의 표시 이름은 별도로 수정할 수 있다.
		// 단말에 설치된 앱 단위로 유니크한 유저키를 생성하는 방법은 아래의 링크를 참고로 한다.
		// DeviceID 를 사용하여도 무방하다.
		// http://blog.naver.com/PostView.nhn?blogId=huewu&logNo=110107222113
		public GBResult LoginWithoutID(string uniqueUserKey, bool forceSync = false) {
			uniqueUserKey = "gbaas_" + uniqueUserKey;
			if (IsAsync(forceSync)) {
				(new Thread(() => this.LoginWithoutIDThread(uniqueUserKey, forceSync))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.LoginWithoutIDThread(uniqueUserKey, forceSync);
			}
		}

		private GBResult LoginWithoutIDThread(string uniqueUserKey, bool forceSync = false) {
			GBResult result = Login(uniqueUserKey, uniqueUserKey, true);

			if (result.isSuccess == false) {
				result = CreateUser(new GBUserObject {
					username = uniqueUserKey,
					password = uniqueUserKey
						//Email = ""
				}, true);

				if (result.isSuccess == true) {
					result = Login(uniqueUserKey, uniqueUserKey, true);
				}
			}

			if (IsAsync(forceSync)) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnLoginWithoutID(result);
				}
			}

			return result;
		}

		public GBResult UpdateUserName(string userName) {
			if (IsAsync()) {
				(new Thread(() => this.UpdateUserNameThread(userName))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.UpdateUserNameThread(userName);
			}
		}

		private GBResult UpdateUserNameThread(string userName) {
			if (IsLogin() == false)
				return new GBResult { isSuccess = false, returnCode = ReturnCode.CheckPreCondition, reason = "Login First" };

			GBResult result = UpdateUser(new GBUserObject {
				username = GetLoginName(),
				name = userName
			});

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnUpdateUserName(result);
				}
			} 

			return result;
		}

		public void Logout() {
			HttpHelper.Instance._accessToken = "";
		}

		public bool IsLogin() {
			return (HttpHelper.Instance._accessToken.Length > 0);
		}

		public GBResult CreateUser(GBUserObject userModel, bool forceSync = false) {
			if (IsAsync() && forceSync == false) {
				(new Thread(() => this.CreateUserThread(userModel))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.CreateUserThread(userModel, forceSync);
			}
		}

		private GBResult CreateUserThread(GBUserObject userModel, bool forceSync = false) {
			string rawResults = "";
			GBResult result = new GBResult();

			try {
				rawResults = GBRequestService.Instance.PerformRequest<string>("/users", HttpHelper.RequestTypes.Post, userModel);
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());

				if (IsAsync() && forceSync == false) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnCreateUser(result);
					}
				}

				return result;
			}

			var entitiesResult = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			if (entitiesResult != null) {

				result.MakeResult(true, ReturnCode.SuccessWithReasonAsResult, entitiesResult[0]["uuid"].ToString());

				if (IsAsync() && forceSync == false) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnCreateUser(result);
					}
				} else {
					return result;
				}
			} else {
				result = UpdateUser(userModel, true);
				result.returnCode = ReturnCode.SuccessButAlt;

				if (IsAsync() && forceSync == false) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnCreateUser(result);
					}
				} else {
					return result;
				}
			}

			return result;
		}

		public GBResult UpdateUser(GBUserObject userModel, bool forceSync = false) {
			if (IsAsync() && forceSync == false) {
				(new Thread(() => this.UpdateUserThread(userModel))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.UpdateUserThread(userModel);
			}
		}

		private GBResult UpdateUserThread(GBUserObject userModel, bool forceSync = false) {
			string userKey = userModel.username;
			if (userModel.uuid != null && userModel.uuid.Length > 0) {
				userKey = userModel.uuid;
			}

			if (userModel.name == null || userModel.name.Length == 0) {
				userModel.name = userModel.username;
			}

			GBResult result = new GBResult();

			string rawResults = "";
			try {
				rawResults = GBRequestService.Instance.PerformRequest<string>(
					"/users/" + userKey, HttpHelper.RequestTypes.Put, userModel);
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());
				if (IsAsync() && forceSync == false) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnUpdateUser(result);
					}
				}
				return result;
			}

			if (HttpHelper.Instance.CheckSuccess(rawResults)) {
				result.MakeResult(true, ReturnCode.SuccessWithReasonAsResult, rawResults.ToString());
			} else {
				result.MakeResult(false, ReturnCode.FailWithReason, rawResults.ToString());
			}

			if (IsAsync() && forceSync == false) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnUpdateUser(result);
				}
			} 

			return result;
		}

		public GBResult LoginWithoutIDUpdate(string uniqueUseKey, GBUserObject userInfo) {
			if (IsAsync()) {
				(new Thread(() => this.LoginWithoutIDUpdateThread(uniqueUseKey, userInfo))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.LoginWithoutIDUpdateThread(uniqueUseKey, userInfo);
			}
		}

		public GBResult LoginWithoutIDUpdateThread(string uniqueUseKey, GBUserObject userInfo) {
			GBResult result = new GBResult();

			// TODO Rest Code is not Asyncalbe, Make Check right away.
			try {
				GBResult loginResult = LoginWithoutID(uniqueUseKey, true);
				if(loginResult.isSuccess == false) {
					result.MakeResult(false, ReturnCode.FailWithReason, "No match user exist with uniqueUseKey(" + uniqueUseKey + ")");
				} else {
					var userInfoResult = GetUserInfo(true);
					if(userInfoResult == null) {
						result.MakeResult(false, ReturnCode.FailWithReason, "Fail GetUserInfo");
					} else {
						userInfo.uuid = userInfoResult.uuid;

						var updateUserResult = UpdateUser(userInfo, true);
						if(updateUserResult == null) {
							result.MakeResult(false, ReturnCode.FailWithReason, "Fail UpdateUser");
						} else {
							// Login With New User Info.
							var loginAgainResult = Login(userInfo.username, "gbaas_" + uniqueUseKey, true);
							if(loginAgainResult.isSuccess == false) {
								result.MakeResult(false, ReturnCode.FailWithReason, "Fail Login Again");
							} else {
								// Change Password
								var changePasswordResult = ChangePassword("gbaas_" + uniqueUseKey, userInfo.password, true);
								if(changePasswordResult.isSuccess == false) {
									result.MakeResult(false, ReturnCode.FailWithReason, "Fail ChangePassword");
								} else {
									result.MakeResult(true, ReturnCode.Success, "");
								}
							}
						}
					}
				}
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());
			}

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnLoginWithoutIDUpdate(result);
				}
			}
			return result;
		}

		public GBResult ChangePassword(string oldOne, string newOne, bool forceSync = false) {
			if (IsAsync(forceSync)) {
				(new Thread(() => this.ChangePasswordThread(oldOne, newOne))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.ChangePasswordThread(oldOne, newOne);
			}
		}

		private GBResult ChangePasswordThread(string oldOne, string newOne, bool forceSync = false) {

			UserModPW passwordObj = new UserModPW {
				newpassword = newOne,
				oldpassword = oldOne
			};

			GBResult result = new GBResult();
			string rawResults = "";

			try {
				rawResults = GBRequestService.Instance.PerformRequest<string>(
					"/users/" + GetLoginName() + "/password", HttpHelper.RequestTypes.Put, passwordObj);
				result.MakeResult(true, ReturnCode.SuccessWithReasonAsResult, rawResults.ToString());
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());
			}

			if (IsAsync(forceSync)) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnChangePassword(result);
				}
			}

			return result;
		}

		public GBUserObject GetUserInfo(bool forceSync = false) {
			if (IsLogin() == false)
				return null;

			if (IsAsync(forceSync)) {
				(new Thread(() => this.GetUserInfoThread())).Start();
				return default(GBUserObject);
			} else {
				return this.GetUserInfoThread();
			}
		}

		private GBUserObject GetUserInfoThread(bool forceSync = false) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/?access_token=" + HttpHelper.Instance._accessToken);
			var user = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (user != null) {
				if (IsAsync(forceSync)) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetUserInfo(MakeUserInfo(user));
					}
				} else {
					return MakeUserInfo(user);
				}
			}

			return default(GBUserObject);
		}

		public List<GBUserObject> GetUserList() {
			if (IsAsync()) {
				(new Thread(() => this.GetUserListThread())).Start();
				return default(List<GBUserObject>);
			} else {
				return this.GetUserListThread();
			}
		}

		private List<GBUserObject> GetUserListThread() {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetUserList(MakeUserList(users));
				}
			} else {
				return MakeUserList(users);
			}

			return default(List<GBUserObject>);
		}

		public List<GBUserObject> GetFollowers() {
			if (IsAsync()) {
				(new Thread(() => this.GetFollowersThread())).Start();
				return default(List<GBUserObject>);
			} else {
				return this.GetFollowersThread();
			}
		}

		private List<GBUserObject> GetFollowersThread() {
			if (!IsLogin()) {
				return default(List<GBUserObject>);
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/followers");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
		
			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetUserList(MakeUserList(users));
				}
			} else {
				return MakeUserList(users);
			}

			return default(List<GBUserObject>);
		}

		public List<GBUserObject> GetFollowing() {
			if (IsAsync()) {
				(new Thread(() => this.GetFollowingThread())).Start();
				return default(List<GBUserObject>);
			} else {
				return this.GetFollowingThread();
			}
		}

		private List<GBUserObject> GetFollowingThread() {
			if (!IsLogin()) {
				return default(List<GBUserObject>);
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/me/following");
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetUserList(MakeUserList(users));
				}
			} else {
				return MakeUserList(users);
			}

			return default(List<GBUserObject>);
		}

		public bool FollowUser(GBUserObject userModel) {
			if (IsAsync()) {
				(new Thread(() => this.FollowUserThread(userModel))).Start();
				return false;
			} else {
				return this.FollowUserThread(userModel);
			}
		}

		private bool FollowUserThread(GBUserObject userModel) {
			if (!IsLogin()) {
				return false;
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/users/me/following/users/" + userModel.username, HttpHelper.RequestTypes.Post, "");

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnFollowUser(HttpHelper.Instance.CheckSuccess(rawResults));
				}
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public bool CreateGroup(GBGroupObject groupModel) {
			if (IsAsync()) {
				(new Thread(() => this.CreateGroupThread(groupModel))).Start();
				return false;
			} else {
				return this.CreateGroupThread(groupModel);
			}
		}

		private bool CreateGroupThread(GBGroupObject groupModel) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/group", HttpHelper.RequestTypes.Post, groupModel);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnCreateGroup(HttpHelper.Instance.CheckSuccess(rawResults));
				}
			} else {
				return HttpHelper.Instance.CheckSuccess(rawResults);
			}

			return false;
		}

		public bool AddUserToGroup(string userName, string groupID) {
			if (IsAsync()) {
				(new Thread(() => this.AddUserToGroupThread(userName, groupID))).Start();
				return false;
			} else {
				return this.AddUserToGroupThread(userName, groupID);
			}
		}

		private bool AddUserToGroupThread(string userName, string groupID) {
			string requestUrl = "/group/" + groupID + "/users/" + userName;

			var rawResults = GBRequestService.Instance.PerformRequest<string> (requestUrl, HttpHelper.RequestTypes.Post, "");

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnAddUserToGroup(HttpHelper.Instance.CheckSuccess (rawResults));
				}
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public bool RemoveUserFromGroup(string userName, string groupID) {
			if (IsAsync()) {
				(new Thread(() => this.RemoveUserFromGroupThread(userName, groupID))).Start();
				return false;
			} else {
				return this.RemoveUserFromGroupThread(userName, groupID);
			}
		}

		private bool RemoveUserFromGroupThread(string userName, string groupID) {
			string requestUrl = "/group/" + groupID + "/users/" + userName;

			var rawResults = GBRequestService.Instance.PerformRequest<string> (requestUrl, HttpHelper.RequestTypes.Delete, "");

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnRemoveUserFromGroup(HttpHelper.Instance.CheckSuccess (rawResults));
				}
			} else {
				return HttpHelper.Instance.CheckSuccess (rawResults);
			}

			return false;
		}

		public List<GBUserObject> GetUsersForGroup(string groupID) {
			if (IsAsync()) {
				(new Thread(() => this.GetUsersForGroupThread(groupID))).Start();
				return default(List<GBUserObject>);
			} else {
				return this.GetUsersForGroupThread(groupID);
			}
		}

		private List<GBUserObject> GetUsersForGroupThread(string groupID) {
			string requestUrl = "/group/" + groupID + "/users";
			
			var rawResults = GBRequestService.Instance.PerformRequest<string>(requestUrl);
			var users = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetUsersForGroup(MakeUserList (users));
				}
			} else {
				return MakeUserList (users);
			}

			return default(List<GBUserObject>);
		}

		private List<GBUserObject> MakeUserList(Newtonsoft.Json.Linq.JToken users) {
			List<GBUserObject> results = new List<GBUserObject>();
			foreach (var usr in users)
			{
				results.Add(new GBUserObject { 
					uuid = (usr["uuid"] ?? "").ToString(),
					name = (usr["name"] ?? "").ToString(),
					username = (usr["username"] ?? "").ToString(),
					password = (usr["password"] ?? "").ToString(),
					lastname = (usr["lastname"] ?? "").ToString(),
					firstname = (usr["firstname"] ?? "").ToString(),
					title = (usr["title"] ?? "").ToString(),
					email = (usr["email"] ?? "").ToString(),
					tel = (usr["tel"] ?? "").ToString(),
					homePage = (usr["homepage"] ?? "").ToString(),
					bday = (usr["bday"] ?? "").ToString(),
					picture = (usr["picture"] ?? "").ToString(),
					url = (usr["url"] ?? "").ToString()
				});
			}

			return results;
		}

		private GBUserObject MakeUserInfo(Newtonsoft.Json.Linq.JToken userToken) {
			var user = userToken[0];
			GBUserObject result = new GBUserObject { 
				uuid = (user["uuid"] ?? "").ToString(),
				name = (user["name"] ?? "").ToString(),
				username = (user["username"] ?? "").ToString(),
				password = (user["password"] ?? "").ToString(),
				lastname = (user["lastname"] ?? "").ToString(),
				firstname = (user["firstname"] ?? "").ToString(),
				title = (user["title"] ?? "").ToString(),
				email = (user["email"] ?? "").ToString(),
				tel = (user["tel"] ?? "").ToString(),
				homePage = (user["homepage"] ?? "").ToString(),
				bday = (user["bday"] ?? "").ToString(),
				picture = (user["picture"] ?? "").ToString(),
				url = (user["url"] ?? "").ToString()
			};

			return result;
		}
	}
}
