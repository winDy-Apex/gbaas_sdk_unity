using System.Collections.Generic;
using GBaaS.io.Objects;
using System.IO;

namespace GBaaS.io {
	public abstract class GBaaSApiHandler {
		public virtual void OnResult(object result) {}

		// For Achievement Service
		public virtual void OnGetAchievement(List<GBAchievementObject> result) {}
		public virtual void OnGetAchievementByUUIDorName(GBAchievementObject result) {}
		public virtual void OnUpdateAchievement(GBAchievementObject result) {}

		// For Score(Leaderboard) Service
		public virtual void OnAddScore(bool result) {}
		public virtual void OnGetScoreByUuidOrName(List<GBScoreObject> result) {}
		public virtual void OnGetScore(List<GBScoreObject> result) {}
		public virtual void OnGetRank(List<GBScoreObject> result) {}
		public virtual void OnGetScoreLog(List<GBScoreObject> result) {}
		
		// For User Service
		public virtual void OnLogin(bool result) {}
		public virtual void OnLoginWithFaceBook(bool result) {}
		public virtual void OnLoginWithoutID(bool result) {}
		public virtual void OnCreateUser(string result) {}
		public virtual void OnUpdateUser(string result) {}
		public virtual void OnUpdateUserName(bool result) {}
		public virtual void OnGetUserInfo(GBUserObject result) {}
		public virtual void OnGetUserList(List<GBUserObject> result) {}
		public virtual void OnGetFollowers(List<GBUserObject> result) {}
		public virtual void OnGetFollowing(List<GBUserObject> result) {}
		public virtual void OnFollowUser(bool result) {}
		public virtual void OnCreateGroup(bool result) {}
		public virtual void OnAddUserToGroup(bool result) {}
		public virtual void OnRemoveUserFromGroup(bool result) {}
		public virtual void OnGetUsersForGroup(List<GBUserObject> result) {}

		// For Push Service
		public virtual void OnSendMessage(bool result) {}
		public virtual void OnRegisterDevice(bool result) {}
		public virtual void OnIsRegisteredDevice(bool result) {}

		// For GameData Service
		public virtual void OnGameDataSave(bool result) {}
		public virtual void OnGameDataLoad(string result) {}

		// For Collection Service
		public virtual void OnGetList(List<GBObject> result) {}
		public virtual void OnGetListInRange(List<GBObject> result) {}
		public virtual void OnGetObject<T>(List<T> result) {}
		public virtual void OnCreateList(bool result) {}

		// For FileStore Service
		public virtual void OnFileUpload(bool result) {}
		public virtual void OnFileDownload(bool result) {} 
		public virtual void OnGetFileList(List<GBAsset> result) {}

		// For Net Service
		public virtual void OnReceiveData(string recvPacket) {}
	}
}
