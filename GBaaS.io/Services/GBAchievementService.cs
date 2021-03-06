using System;
using System.Collections.Generic;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using GBaaS.io.Utils;
using System.Threading;

namespace GBaaS.io
{
	class GBAchievementService : GBService<GBAchievementService>
	{
		private List<Objects.GBAchievementObject> _achievement = null;

		public GBAchievementService () {}

		public List<GBaaSApiHandler> _handler = new List<GBaaSApiHandler>();

		public void SetHandler(GBaaSApiHandler handler) {
			if (handler == null) {
				_handler.Clear();
			} else {
				_handler.Add(handler);
			}
		}

		private bool IsAsync() {
			return (_handler.Count > 0);
		}

		public GBResult AddUserAchievement(GBObject achievement) {
			return achievement.Save();
		}

		public List<Objects.GBAchievementObject> GetAchievement(string locale = "", int limit = 0, string cursor = "") {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetAchievementThread(locale, limit, cursor));
				workerThread.Start();
				return default(List<Objects.GBAchievementObject>);
			} else {
				return this.GetAchievementThread(locale, limit, cursor);
			}
		}

		public List<Objects.GBAchievementObject> GetAchievementThread(string locale = "", int limit = 0, string cursor = "") {
			string query = "";

			if (locale.Length > 0) {
				query += "locale=" + locale;
			}

			if (limit > 0) {
				if (locale.Length > 0) {
					query += "&";
				}
				query += "limit=" + limit.ToString();
			}

			if (cursor.Length > 0) {
				if ((locale.Length + limit) > 0) {
					query += "&";
				}
				query += "cursor=" + cursor;
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/achievements" + "?" + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults != null && rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetAchievement(default(List<Objects.GBAchievementObject>));
					}
				} else { 
					return default(List<Objects.GBAchievementObject>);
				}
			}

			var achievements = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
		
			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetAchievement(MakeAchievementList(achievements));
				}
			} else {
				return MakeAchievementList(achievements);
			}

			return default(List<Objects.GBAchievementObject>);
		}

		public Objects.GBAchievementObject GetAchievementByUUID(string uuid, string locale = "") {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetAchievementByUUIDThread(uuid, locale));
				workerThread.Start();
				return default(Objects.GBAchievementObject);
			} else {
				return this.GetAchievementByUUIDThread(uuid, locale);
			}
		}

		public Objects.GBAchievementObject GetAchievementByUUIDThread(string uuid, string locale = "") {
			string query = "";

			if (locale.Length > 0) {
				query += "locale=" + locale;
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/achievement/" + uuid + "?" + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetAchievementByUUIDorName(default(Objects.GBAchievementObject));
					}
				} else {
					return default(Objects.GBAchievementObject);
				}
			}

			var achievements = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetAchievementByUUIDorName(MakeAchievementList(achievements)[0]);
				}
			} else {
				return MakeAchievementList(achievements)[0];
			}

			return default(Objects.GBAchievementObject);
		}

		/// <summary>
		/// Updates the achievement.
		/// </summary>
		/// <returns>The achievement.</returns>
		/// <param name="currentStepCount">Current step count.</param>
		/// <param name="isUnlocked">If set to <c>true</c> is unlocked.</param>
		public Objects.GBAchievementObject UpdateAchievement(string uuid, int currentStepCount, bool isUnlocked, string locale = "") {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.UpdateAchievementThread(uuid, currentStepCount, isUnlocked, locale));
				workerThread.Start();
				return default(Objects.GBAchievementObject);
			} else {
				return this.UpdateAchievementThread(uuid, currentStepCount, isUnlocked, locale);
			}
		}

		public Objects.GBAchievementObject UpdateAchievementThread(string uuid, int currentStepCount, bool isUnlocked, string locale) {
			Objects.GBUserAchievementObject userAchievementObject = new Objects.GBUserAchievementObject {
				currentStepCount = currentStepCount,
				isUnLocked = isUnlocked
			};

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/users/" + GBUserService.Instance.GetLoginName() + "/achievements/" + uuid + "?locale=" + locale, HttpHelper.RequestTypes.Put, userAchievementObject);
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnUpdateAchievement(default(Objects.GBAchievementObject));
					}
				} else {
					return default(Objects.GBAchievementObject);
				}
			}

			var achievements = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnUpdateAchievement(MakeAchievementList(achievements)[0]);
				}
			} else {
				return MakeAchievementList(achievements)[0];
			}

			return default(Objects.GBAchievementObject);
		}

		private List<Objects.GBAchievementObject> MakeAchievementList(Newtonsoft.Json.Linq.JToken achievements) {
			_achievement = new List<Objects.GBAchievementObject>();
			if (achievements == null) {
				return default(List<Objects.GBAchievementObject>);
			}

			foreach (var item in achievements)
			{
				_achievement.Add(new Objects.GBAchievementObject { 
					uuid = (item["uuid"] ?? "").ToString(),
					incrementalCount = Convert.ToInt32( (item["incrementalCount"] ?? "").ToString() ),
					isMoreThanOnce = Convert.ToBoolean( (item["isMoreThanOnce"] ?? "").ToString() ),
					isHidden = Convert.ToBoolean( (item["isHidden"] ?? "").ToString() ),
					points = Convert.ToInt32( (item["points"] ?? "").ToString() ),
					achievementName = (item["name"] ?? "").ToString(),
					preEarnedDescription = (item["preEarnedDescription"] ?? "").ToString(),
					earnedDescription = (item["earnedDescription"] ?? "").ToString(),
					currentStepCount = Convert.ToInt32( (item["currentStepCount"] ?? "0").ToString() ),
					isUnLocked = HttpHelper.Instance.SafeConvertBoolean((item["isUnLocked"] ?? "false").ToString() )
				});
			}

			return _achievement;
		}
	}
}
