using System;
using NUnit.Framework;
using GBaaS.io;
using System.Collections.Generic;
using GBaaS.io.Objects;

namespace GBaaS.io.Tests
{
	public class UserHandler : GBaaSApiHandler {
		/*public override void OnResult(object result) {
			Console.Out.WriteLine("Async On Result : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
		}
		*/

		public override void OnGetAchievement(List<Objects.GBAchievementObject> result) {
			Console.Out.WriteLine("Async On GetAchievement : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.Count > 0);
		}

		public override void OnGetAchievementByUUIDorName(Objects.GBAchievementObject result) {
			Console.Out.WriteLine("Async On GetAchievementByUUIDorName : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.achievementName.Length > 0);
		}

		public override void OnUpdateAchievement(Objects.GBAchievementObject result) {
			Console.Out.WriteLine("Async On UpdateAchievement : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.achievementName.Length > 0);
		}

		public override void OnAddScore(bool result) {
			Console.Out.WriteLine("Async On AddScore : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result);
		}

		public override void OnGetScoreByUuidOrName(List<Objects.GBScoreObject> result) {
			Console.Out.WriteLine("Async On GetScoreByUuidOrName : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsNotNull(result);
		}

		public override void OnGetScore(List<Objects.GBScoreObject> result) {
			Console.Out.WriteLine("Async On GetScore : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsNotNull(result);
		}

		public override void OnGetScoreLog(List<Objects.GBScoreObject> result) {
			Console.Out.WriteLine("Async On GetScoreLog : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsNotNull(result);
		}
	}

	abstract class Singleton<DerivedType>
		where DerivedType : new()
	{
		private static DerivedType _instance;

		public static DerivedType Instance
		{
			get
			{
				if (_instance == null) {
					_instance = new DerivedType ();
				}

				return _instance;
			}
		}
	}

	class AsyncCallChecker : Singleton<AsyncCallChecker>
	{
		public bool _isAsyncCalling = false;

		public void SetAsyncCalling(bool isAsyncCalling) {
			_isAsyncCalling = isAsyncCalling;
		}

		public bool GetAsyncCalling() {
			return _isAsyncCalling;
		}
	}

	public class AsyncTests
	{
		[Test]
		public void SetHandlerTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);

			Assert.IsFalse(aClient2.IsAsync());

			GBaaSApiHandler handler = new UserHandler();

			aClient2.SetHandler(handler);

			Assert.IsTrue(aClient2.IsAsync());

			aClient2.SetHandler(null);

			Assert.IsFalse(aClient2.IsAsync());
		}

		[Test]
		public void CallAsyncGetAchievement()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient.Login("test", "abc123");

			String locale = "ko-KR";

			GBaaSApiHandler handler = new UserHandler();
			aClient.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient.GetAchievement(locale, 50, "");
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient.SetHandler(null);
		}

		[Test]
		public void CallAsyncGetAchievementByUUIDorName()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			String locale = "ko-KR";

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.GetAchievementByUUIDorName("GetScore2000Over", locale);
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}

		[Test]
		public void CallAsyncUpdateAchievement()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.UpdateAchievement("GetScore2000Over", 10, true, "ko-KR");
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}

		[Test]
		public void CallAsyncAddScore() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);

			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			// Name must be unique.
			var result = aClient2.AddScore (new GBScoreObject {
				stage = "1st",
				score = 100,
				unit = "point" 
			});
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == false);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}

		[Test]
		public void CallAsyncGetScoreByUuidOrName() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient2.GetScore("1st", "point", 0, "");

			Console.Out.WriteLine("GetScoreTest Result Count : " + result.Count.ToString());
			Assert.IsNotNull(result);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			Console.Out.WriteLine("GetScoreTest Result UUID : " + result[0].GetUUID());
			List<Objects.GBScoreObject> result2 = aClient2.GetScoreByUuidOrName(result[0].GetUUID());

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result2 == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}

		[Test]
		public void CallAsyncGetScore() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			List<Objects.GBScoreObject> result = aClient2.GetScore("1st", "point", 0, "");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}

		[Test]
		public void CallAsyncGetScoreLog() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.SetHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			List<Objects.GBScoreObject> result = aClient2.GetScoreLog("1st", "point", 0, "");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.SetHandler(null);
		}
	}
}