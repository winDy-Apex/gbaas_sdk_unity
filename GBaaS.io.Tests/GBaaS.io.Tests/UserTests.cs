using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using NUnit.Framework;
using GBaaS.io.Utils;

namespace GBaaS.io.Tests
{
	public class UserTests
	{
		GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL3);
		//string _token = "";

		[Test]
		public void LoginTest()
		{
			string loginUser = Defines.TEST_USERNAME;
			//string loginUser = "gbaas_d85f7a6c-c04f-4a38-bd80-6a3e74b2dc23"; //037281ae-2fa3-48d3-875a-2ac3d99416c1";
			var results = aClient.Login(loginUser, Defines.TEST_PASSWORD);
			Assert.IsTrue(results.isSuccess);
		}		

		[Ignore]
		[Test]
		public void LoginUsingNoID()
		{
			string loginUser = "gbaas_9e5424e3-95f1-429a-b576-d62bc5e49880";
			var results = aClient.Login(loginUser, loginUser);
			Assert.IsTrue(results.isSuccess);
		}

		[Ignore]
		[Test]
		public void LoginWithFaceBook()
		{
			string userToken = "CAADqYZChB3jUBAEU2gUrNrrCrgUy6jXFZAJyjWiN8ArO0kBLtf6WuNsg6CVZB61XWQfZAJZB71gvHJld7jTy3lpoZAI3FmxlFZC9DArJGkPLs3wTwPhr9PAJ5HHQGr4CPoTco9GLsLL6VxuTNYzporjg9r6vlaVX5lC74ffD4y1dlc1pkPSIOMzhbKUhw6xf83XaDuJ4oe9dH93rEOp5TJY";

			var results = aClient.LoginWithFaceBook(userToken);
			Assert.IsTrue(results.isSuccess);
		}

		[Test]
		public void LoginWithoutID()
		{
			string un = Guid.NewGuid().ToString();
			// Why next line code fail? I don't know...
			//var result = aClient.LoginWithoutID("ec6af9bd-b05d-4ce5-9d23-cb1a1a51ed98");

			//비동기 호출은 내부적으로 새 아이디 생성을 완료 후 리턴하므로 중간 과정의 False 리턴이 없다.
			//See. AsyncTests.cs::CallAsyncLoginWithoutID
			var result = aClient.LoginWithoutID(un);
			Assert.IsTrue(result.isSuccess);
		}


		[Test]
		public void LoginWithoutID_FixedString() {
			var result = aClient.LoginWithoutID("ABABABABABCCCCCCCCDDDD12345");
			Assert.IsTrue(result.isSuccess);
		}

		[Ignore]
		[Test]
		public void LoginWithoutIDUpdate() {
			var result = aClient.LoginWithoutIDUpdate("ABABABABABCCCCCCCCDDDD12345", new GBUserObject {
				username = "ChangeIDABABABABABCCCCCCCCDDDD12345",
				password = Defines.TEST_PASSWORD_CHANGE,
				email = "changetest@test.com",
				age = 19,
				gender = "Female"
			});

			Assert.IsTrue(result);

			var results = aClient.Login("ChangeIDABABABABABCCCCCCCCDDDD12345", Defines.TEST_PASSWORD_CHANGE);
			Assert.IsTrue(results.isSuccess);
		}

		[Ignore]
		[Test]
		public void LoginWithoutIDUpdateAfter() {
			var results = aClient.Login("ChangeIDABABABABABCCCCCCCCDDDD12345", Defines.TEST_PASSWORD_CHANGE);
			Assert.IsTrue(results.isSuccess);
		}

		[Test]
		public void LoginWithoutIDAndChangeIDAfter()
		{
			string un = Guid.NewGuid().ToString();
			//string un = "TTOOMM";
			var result = aClient.LoginWithoutID(un);
			Assert.IsTrue(result.isSuccess);

			var results = aClient.GetUserInfo();
			Assert.IsNotNull(results);

			var resultss = aClient.UpdateUser(new GBUserObject {
				uuid = results.uuid,
				username = "ChangeID" + un,
				password = Defines.TEST_PASSWORD,
				email = un + "@test.com",
				age = 19,
				gender = "Female"
			});
			Assert.IsNotNull (resultss);

			// Notice! Password Unchanged. It needs another API for changing password. UpdateUser call Can't do that.
			var result2 = aClient.Login("ChangeID" + un, "gbaas_" + un); //Defines.TEST_PASSWORD);
			Assert.IsTrue(result2.isSuccess);
		}

		[Test]
		public void UpdateUserName()
		{
			string un = "gbaas_" + Guid.NewGuid();
			//string un = "gbaas_1234567";
			var result = aClient.LoginWithoutID(un);
			Assert.IsTrue(result.isSuccess);

			string name = "John Smith";
			var updateresult = aClient.UpdateUserName(name);
			Assert.IsTrue(updateresult);
		}

		[Test]
		public void LoginFailTest()
		{
			var results = aClient.Login(Defines.TEST_USERNAME, "");
			Assert.IsFalse(results.isSuccess);
		}
		
		[Test]
		public void CreateUserTest()
		{
			//string un = Defines.TEST_USERNAME;
			string un = "gbaas_" + Guid.NewGuid();
			//string un = "ChangeID";

			var result = aClient.CreateUser(new GBUserObject {
				username = un,
				password = Defines.TEST_PASSWORD,
				email = un + "@test.com"
			});

			Console.WriteLine(result);
			Assert.IsNotEmpty (result.reason); // by winDy // .Uuid);
		}

		[Test]
		public void UpdateUserTest()
		{
			string un = "test1";
			var result2 = aClient.Login(un, Defines.TEST_PASSWORD);
			Assert.IsTrue(result2.isSuccess);

			var result = aClient.UpdateUser(new GBUserObject {
				username = "bobby",
				password = Defines.TEST_PASSWORD + "1",
				email = un + "@test1112.com",
				age = 19,
				gender = "Female"
			});
			Assert.IsNotNull (result);

			// Can't change the password. It needs special api.
			var result3 = aClient.Login(un, Defines.TEST_PASSWORD);
			Assert.IsTrue(result3.isSuccess);

			var result4 = aClient.UpdateUser(new GBUserObject {
				username = un,
				password = Defines.TEST_PASSWORD,
				email = un + "@test123.com",
				age = 25,
				gender = "Male"
			});
			Assert.IsNotNull (result4);
		}

		[Test]
		public void ChangePassword() {
			var result = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsTrue(result.isSuccess);

			var result2 = aClient.ChangePassword(Defines.TEST_PASSWORD, Defines.TEST_PASSWORD_CHANGE);
			Assert.IsNotNull (result2);

			var result3 = aClient.ChangePassword(Defines.TEST_PASSWORD_CHANGE, Defines.TEST_PASSWORD);
			Assert.IsNotNull (result3);
		}

		[Test]
		public void GetUserInfoTest()
		{
			var result = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsTrue(result.isSuccess);

			var results = aClient.GetUserInfo();
			Assert.IsNotNull(results);
		}

		[Test]
		public void GetUserListTest()
		{
			var results = aClient.GetUserList();
			Assert.IsNotNull(results);
		}

		[Test]
		public void GetFollwersTest()
		{
			var result = aClient.GetFollowers ();
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetFollowingTest()
		{
			var result = aClient.GetFollowing ();
			Assert.IsNotNull (result);
		}

		[Test]
		public void FollowUserTest()
		{
			LoginTest ();
			var result = aClient.FollowUser (new GBUserObject {
				username = "bobby"
			});
			Assert.IsTrue (result);
		}

		[Test]
		public void CreateGroupTest()
		{
			var results = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsTrue(results.isSuccess);
			var result = aClient.CreateGroup (new GBGroupObject {
				path = "groupPath" + Guid.NewGuid(),
				title = "test group"
			});
			Assert.IsTrue (result);
		}

		[Test]
		public void AddUserToGroupTest()
		{
			var results = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsTrue(results.isSuccess);
			var result = aClient.AddUserToGroup ("bobby", "groupPath");
			Assert.IsTrue (result);
		}

		[Test]
		public void RemoveUserFromGroupTest()
		{
			var results = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsTrue(results.isSuccess);
			var result = aClient.RemoveUserFromGroup ("bobby", "groupPath");
			Assert.IsTrue (result);
		}

		[Test]
		public void GetUsersForGroupTest()
		{
			var result = aClient.GetUsersForGroup ("groupPath");
			Assert.IsNotNull (result);
		}
	}
}
