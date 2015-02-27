using System;
using NUnit.Framework;
using GBaaS.io;

namespace GBaaS.io.Tests
{
	public class AchievementTests
	{
		GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
		//string _token = "";

		[Test]
		public void SafeConvertBoolean()
		{
			string booleanString = "True";
			bool result = booleanString.ToLower().Equals("true");
			Assert.IsTrue(result);
			booleanString = "true";
			result = booleanString.ToLower().Equals("true");
			Assert.IsTrue(result);
			booleanString = "False";
			result = booleanString.ToLower().Equals("true");
			Assert.IsFalse(result);
			booleanString = "false";
			result = booleanString.ToLower().Equals("true");
			Assert.IsFalse(result);
			booleanString = "";
			result = booleanString.ToLower().Equals("true");
			Assert.IsFalse(result);
		}

		[Test]
		//[Ignore] // Achievement 등록은 웹을 통해서 이루어져야한다. 테스트를 위해서 임시로 1회만 등록 확인을 위해 사용하였다.
		public void SaveAchievementTest()
		{
			aClient.Login("test", "abc123");
			Objects.GBAchievementInfoObject achievement = new Objects.GBAchievementInfoObject {
				name = "Get Score 2500 Over",
				incrementalCount = 1,
				isMoreThanOnce = false,
				isHidden = false,
				points = 0,
				listOrder = 1,
				processStatus = 2
			};

			Assert.IsTrue(achievement.Save().isSuccess);

			Objects.GBAchievementInfoObject achievement2 = new Objects.GBAchievementInfoObject {
				name = "Strike Down Red Building First",
				incrementalCount = 1,
				isMoreThanOnce = false,
				isHidden = false,
				points = 0,
				listOrder = 2,
				processStatus = 2
			};

			Assert.IsTrue(achievement2.Save().isSuccess);
		}

		[Test]
		//[Ignore] // Achievement 등록은 웹을 통해서 이루어져야한다. 테스트를 위해서 임시로 1회만 등록 확인을 위해 사용하였다.
		public void SaveAchievementLocaleTest()
		{
			aClient.Login("test", "abc123");
			Objects.GBAchievementLocaleObject achievementObject = new Objects.GBAchievementLocaleObject {
				name = "08c54fd4-8ef4-11e3-b35b-5b27d4c75a66-ko-KR", // AchievementName + "-" + LocaleId
				achievementId = "08c54fd4-8ef4-11e3-b35b-5b27d4c75a66",
				localeId = "ko-KR",
				achievementName = "Strike Down Red Building First",
				preEarnedDescription = "빨간색 건물을 먼저 파괴하시오.",
				earnedDescription = "빨간색 건물의 파괴자.",
				isDefaultLanguage = true
			};

			Assert.IsTrue(achievementObject.Save().isSuccess);

			Objects.GBAchievementLocaleObject achievementObject2 = new Objects.GBAchievementLocaleObject {
				name = "08e3862a-8ef4-11e3-af28-c92aeafdcd01-ko-KR", // AchievementName + "-" + LocaleId
				achievementId = "08e3862a-8ef4-11e3-af28-c92aeafdcd01",
				localeId = "ko-KR",
				achievementName = "Get Score 2500 Over",
				preEarnedDescription = "2500점을 획득하시오.",
				earnedDescription = "탱크 마스터.",
				isDefaultLanguage = true
			};

			Assert.IsTrue(achievementObject2.Save().isSuccess);
		}

		[Test]
		//[Ignore] // Achievement 등록은 웹을 통해서 이루어져야한다. 테스트를 위해서 임시로 1회만 등록 확인을 위해 사용하였다.
		public void SaveAchievementGBaaSManTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			Objects.GBAchievementInfoObject achievement = new Objects.GBAchievementInfoObject {
				name = "GetScore2000Over",
				incrementalCount = 1,
				isMoreThanOnce = false,
				isHidden = false,
				points = 0,
				listOrder = 1,
				processStatus = 2
			};

			Assert.IsTrue(achievement.Save().isSuccess);

			Objects.GBAchievementInfoObject achievement2 = new Objects.GBAchievementInfoObject {
				name = "UseBombMoreThanOnce",
				incrementalCount = 1,
				isMoreThanOnce = false,
				isHidden = false,
				points = 0,
				listOrder = 2,
				processStatus = 2
			};

			Assert.IsTrue(achievement2.Save().isSuccess);

			aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
		}

		/*
		[Test]
		[Ignore] // Achievement 등록은 웹을 통해서 이루어져야한다. 테스트를 위해서 임시로 1회만 등록 확인을 위해 사용하였다.
		public void SaveAchievementLocaleGBaaSManTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			Objects.GBAchievementLocaleObject achievementObject = new Objects.GBAchievementLocaleObject {
				name = Defines.GBAASMAN2_ACHIEVEMENT1 + "-ko-KR", // AchievementId + "-" + LocaleId
				achievementId = Defines.GBAASMAN2_ACHIEVEMENT1,
				localeId = "ko-KR",
				achievementName = "UseBombMoreThanOnce",
				preEarnedDescription = "폭탄을 한번 이상 사용하시오.",
				earnedDescription = "봄버맨.",
				isDefaultLanguage = true
			};

			Assert.IsTrue(achievementObject.Save());

			Objects.GBAchievementLocaleObject achievementObject2 = new Objects.GBAchievementLocaleObject {
				name = Defines.GBAASMAN2_ACHIEVEMENT2 + "-ko-KR", // AchievementId + "_" + LocaleId
				achievementId = Defines.GBAASMAN2_ACHIEVEMENT2,
				localeId = "ko-KR",
				achievementName = "GetScore2000Over",
				preEarnedDescription = "2000점을 획득하시오.",
				earnedDescription = "외계인 킬러.",
				isDefaultLanguage = true
			};

			Assert.IsTrue(achievementObject2.Save());

			aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
		}
		*/

		[Test]
		public void SaveUserAchievementTest()
		{
			String username = "gbaas_" + Guid.NewGuid();
			String achievementName = "Get Score 2500 Over_ko-KR";

			Objects.GBUserAchievementObject userAchievementObject = new Objects.GBUserAchievementObject {
				name = username + "_" + achievementName,
				userName = username,
				achievementId = "e0aa623a-6df3-11e3-9e30-37800d66ccba",
				currentStepCount = 1,
				isUnLocked = true
			};

			var result = userAchievementObject.Save();
			Assert.IsTrue(result.isSuccess);
		}

		[Test]
		public void SaveUserAchievementViaServiceTest()
		{
			String username = "gbaas_" + Guid.NewGuid();
			String achievementName = "Get Score 2500 Over_ko-KR";

			Objects.GBUserAchievementObject userAchievementObject = new Objects.GBUserAchievementObject {
				name = username + "_" + achievementName,
				userName = username,
				achievementId = "e0aa623a-6df3-11e3-9e30-37800d66ccba",
				currentStepCount = 1,
				isUnLocked = true
			};

			var result = aClient.AddUserAchievement(userAchievementObject);
			Assert.IsTrue(result.isSuccess);
		}

		[Test]
		public void GetAchievementTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			aClient2.Login("test", "abc123");

			String locale = "ko-KR";

			var result = aClient.GetAchievement(locale, 50, "");
			Assert.IsTrue(result.Count > 0);
		}

		[Test]
		public void GetAchievementGBaaSManTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			String locale = "ko-KR";

			var result = aClient.GetAchievement(locale, 50, "");
			Assert.IsTrue(result.Count > 0);
		}


		[Test]
		public void GetAchievementByUUID()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			String locale = "ko-KR";

			var result = aClient.GetAchievementByUUID(Defines.TEST_ACHIEVEMENT_UUID, locale);
			Assert.IsTrue(result.achievementName.Length > 0);
		}

		[Test]
		public void UpdateAchievement()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			var result = aClient.UpdateAchievement(Defines.TEST_ACHIEVEMENT_UUID, 1, true, "ko-KR");
			Assert.IsTrue(result.achievementName.Length > 0);
		}

		[Test]
		public void GetEntityTypeTest()
		{
			Objects.GBAchievementInfoObject achievement = new Objects.GBAchievementInfoObject();
			string result = achievement.GetEntityType();
			Assert.IsTrue("achievementinfo".Equals(result));
		}
	}
}
