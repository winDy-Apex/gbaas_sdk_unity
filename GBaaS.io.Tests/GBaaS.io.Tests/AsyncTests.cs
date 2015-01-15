using System;
using NUnit.Framework;
using GBaaS.io;
using System.Collections.Generic;
using GBaaS.io.Objects;

namespace GBaaS.io.Tests
{
	/*
	public class CustomOneObject : GBObject {
		public string mydataOne { get; set; }
		public string mydataTwo { get; set; }
		public string mydataThree { get; set; }
	}
	*/

	public class UserHandler : GBaaSApiHandler {
		/*public override void OnResult(object result) {
			Console.Out.WriteLine("Async On Result : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);
		}
		*/

		public override void OnLogin(GBResult result) {
			Console.Out.WriteLine("Async On Login : returnCode(" + result.returnCode + ") reason(" + result.reason + ")");
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.isSuccess);
		}

		public override void OnLoginWithoutID(GBResult result) {
			Console.Out.WriteLine("Async On OnLoginWithoutID : returnCode(" + result.returnCode + ") reason(" + result.reason + ")");
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.isSuccess);
		}

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

		public override void OnCreateUser(GBResult result) {
			Console.Out.WriteLine("Async On CreateUser : " + result.reason);
			AsyncCallChecker.Instance.SetAsyncCalling(false);
			Assert.IsTrue(result.isSuccess);
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

		public override void OnGetObject<T>(List<T> result) {
			Console.Out.WriteLine("Async On GetObject Count is " + result.Count.ToString());
			Console.Out.WriteLine("Custom Object Type is " + GBCollectionService.Instance.GetTypeName(typeof(T)));

			string customObjectType = GBCollectionService.Instance.GetTypeName(typeof(T));

			if (customObjectType.CompareTo("CustomOneObject") == 0) {
				List<CustomOneObject> collection = (List<CustomOneObject>)((object)result);

				AsyncCallChecker.Instance.SetAsyncCalling(false);

				Assert.IsTrue(collection[0].mydataOne.CompareTo("One Data") == 0);
			}
		}

		public override void OnIsRegisteredDevice(bool result) {
			Console.Out.WriteLine("Async On IsRegisteredDevice : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsTrue(result);
		}

		public override void OnRegisterDevice(bool result) {
			Console.Out.WriteLine("Async On RegisterDevice : " + result.ToString());
			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsTrue(result);
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

			aClient2.AddHandler(handler);

			Assert.IsTrue(aClient2.IsAsync());

			aClient2.AddHandler(null);

			Assert.IsFalse(aClient2.IsAsync());
		}

		[Test]
		public void CallAsyncLoginWithoutID() {

			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			GBaaSApiHandler handler = new UserHandler();
			aClient.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			string un = Guid.NewGuid().ToString();
			var result = aClient.LoginWithoutID(un);

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsFalse(result.isSuccess);

			//Async 호출이 끝날때까지 대기, For Only Test Code, 실제로는 불필요한 코드
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient.AddHandler(null);
		}

		[Test]
		public void CallAsyncLogin() {

			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			GBaaSApiHandler handler = new UserHandler();
			aClient.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result.returnCode == ReturnCode.WaitAsync);

			//Async 호출이 끝날때까지 대기, For Only Test Code, 실제로는 불필요한 코드
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient.AddHandler(null);
		}

		[Test]
		public void CallAsyncGetObject()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient.Login("test", "abc123");

			GBaaSApiHandler handler = new UserHandler();
			aClient.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);
			List<CustomOneObject> collection = aClient.GetObject<CustomOneObject>("mydataOne", "One Data", 1);

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(collection == default(List<CustomOneObject>));
			
			//Async 호출이 끝날때까지 대기, For Only Test Code, 실제로는 불필요한 코드
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
					Console.Out.WriteLine ("...AsyncCalling...");
					System.Threading.Thread.Sleep(100);
			}

			aClient.AddHandler(null);
		}

		[Test]
		public void CallAsyncGetAchievement()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient.Login("test", "abc123");

			String locale = "ko-KR";

			GBaaSApiHandler handler = new UserHandler();
			aClient.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient.GetAchievement(locale, 50, "");
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient.AddHandler(null);
		}


		[Test]
		public void CallAsyncCreateUser()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			//aClient.Login("test", "abc123");

			GBaaSApiHandler handler = new UserHandler();
			aClient.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			string un = "gbaas_" + Guid.NewGuid();
			//string un = "test1";

			var result = aClient.CreateUser(new GBUserObject {
				username = un,
				password = Defines.TEST_PASSWORD,
				email = un + "@test.com"
			});

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result.returnCode == ReturnCode.WaitAsync);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient.AddHandler(null);
		}

		[Test]
		public void CallAsyncGetAchievementByUUID()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			String locale = "ko-KR";

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.GetAchievementByUUID("d0b24e5a-54f3-11e4-b688-3199c630a20e", locale);
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncUpdateAchievement()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.UpdateAchievement("d0b24e5a-54f3-11e4-b688-3199c630a20e", 10, true, "ko-KR");
			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncAddScore() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);

			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

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

			aClient2.AddHandler(null);
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
			aClient2.AddHandler(handler);

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

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncGetScore() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			List<Objects.GBScoreObject> result = aClient2.GetScore("1st", "point", 0, "");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result[0].returnCode == ReturnCode.WaitAsync);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncGetScoreLog() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			List<Objects.GBScoreObject> result = aClient2.GetScoreLog("1st", "point", 0, "");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsTrue(result == null);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncIsRegisteredDevice() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			bool result = aClient2.IsRegisteredDevice("iPhone", "7.1", "iOS", "1234-5678-9000111");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsFalse(result);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncIsRegisteredDevice2() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			//11-10 17:45:50.126: I/Unity(29254): [GBaaS] Registered!!! 

			string registrationId = "APA91bGQxrGBLda4bgPy15qTex-7m9SaxFv8rCO2u2-4lTZNZp4zvGLE8Rz5ZroA9xN3Z2FcMaSbQ4u5KzJdaxm6HV1i3Cyf_hZXoXsXSB0q4aCXvVNkOYAnoJLrfwxPYKr14BDNFT6iVXhPUzvP7mfY_hTVEpPlPSMEe21oG9MNRsCehaXvzFc";
			string deviceModel = "samsung SHV-E250K";
			string operatingSystem = "Android OS 4.4.2 / API-19 (KOT49H/E250KKTUKNI1)";

			bool result = aClient2.IsRegisteredDevice(deviceModel, operatingSystem, "andriod", registrationId);

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsFalse(result);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}

		[Test]
		public void CallAsyncRegisterDevice() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			var login = aClient2.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			bool result = aClient2.RegisterDevice("iPhone", "7.1", "iOS", "1234-5678-9000111");

			//바로 리턴되는 결과는 없어야 정상
			Assert.IsFalse(result);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			aClient2.AddHandler(null);
		}
	}
}
