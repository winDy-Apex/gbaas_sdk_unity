using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using NUnit.Framework;

namespace GBaaS.io.Tests.ClientTests
{
    public class ClientTests
    {
		GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
		string _token = "";

        [Test]
        public void GetTokenTest()
		{
			//var result = aClient.GetToken("apigee_58461c11-6632-4980-9130-cb43fc5d0dc6", "abc123");
			var result = aClient.GetToken(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
            Assert.IsNotNull(result);
			_token = result;
        }

        [Test]
        public void LookUpTokenTest()
        {
			var result = aClient.LookUpToken(_token);
            Assert.IsNotNull(result);
        }

		/*
        public static void temp()
        {
            ApigeeClient apiClient = new ApigeeClient("http://api.usergrid.com/xxx/sandbox/");

            //Get a collection of all users 
            var allUsers = apiClient.GetUsers();

            string un = "apigee_" + Guid.NewGuid();

            //Create a new Account
            apiClient.CreateAccount(new ApigeeUserModel
            {
                Username = un,
                Password = "abc123",
                Email = un + "@test.com"
            });

            //Update an Existing Account
            apiClient.UpdateAccount(new ApigeeUserModel
            {
                Username = un,
                Password = "abc123456",
                Email = un + "@test.com"
            });

            //Login User - Get Token 
            var token = apiClient.GetToken(un, "abc123456");

            //Lookup a user by token ID
            var username = apiClient.LookUpToken(token);
        }
        */
    }

	public class ScoreTests
	{
		GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);

		[Test]
		public void AddScoreTest() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// Name must be unique.
			var result = aClient.AddScore (new GBScoreObject {
				stage = "1st",
				score = 1201,
				unit = "point" 
			});

			Assert.IsTrue(result);
		}

		[Test]
		public void GetScoreAndUpdateScoreAndSetLocation() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient.GetScore ("1st", "point", 0, "");

			Console.Out.WriteLine ("GetScoreTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);

			Console.Out.WriteLine ("GetScoreTest Result UUID : " + result[0].GetUUID());
			List<Objects.GBScoreObject> result2 = aClient.GetScoreByUuidOrName (result[0].GetUUID());

			Console.Out.WriteLine ("GetScoreTest Result2 UUID : " + result2[0].GetUUID());
			Assert.IsTrue(result[0].GetUUID().CompareTo(result2[0].GetUUID()) == 0);

			// Name must be unique.
			GBScoreObject scoreObject = new GBScoreObject {
				stage = "1st",
				score = 190,
				unit = "point"
			};
			scoreObject.SetUUID(result[0].GetUUID());

			var result3 = aClient.AddScore (scoreObject);

			Assert.IsTrue(result3);

			var result4 = scoreObject.SetLocation(1.33f, 2.1f);

			Assert.IsTrue(result4);
		}

		[Test]
		public void GetScoreLogTest() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient.GetScoreLog("1st", "point", 0, "");

			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreLogMoreTest() {
			var login = aClient.Login("test", Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient.GetScoreLogMore("", "", 2, false);

			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);

			result = aClient.GetScoreLogMore("", "", 2, true);

			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreLogWithoutStageUnitInfoTest() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient.GetScoreLog("", "", 0, "");

			Console.Out.WriteLine ("GetScoreLogWithoutStageUnitInfoTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreWeekly() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);
			List<Objects.GBScoreObject> result = aClient.GetScore ("", "", 10, "", GBaaS.io.Period.Weekly, DayOfWeek.Sunday);

			Console.Out.WriteLine ("GetScoreWeekly Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreDaily() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);
			List<Objects.GBScoreObject> result = aClient.GetScore ("", "", 10, "", GBaaS.io.Period.Daily);

			Console.Out.WriteLine ("GetScoreDaily Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreUser() {
			List<Objects.GBScoreObject> result = aClient.GetScoreLog("", "", 10, "", GBaaS.io.Period.Daily);

			Console.Out.WriteLine ("GetScoreUser Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetRank() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = aClient.GetRank("", "", ScoreOrder.DESC, Period.Monthly, 0, 5);

			Console.Out.WriteLine ("GetRank Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void AddScoreTestWithStage() {
			var login = aClient.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// Name must be unique.
			var result = aClient.AddScore (new GBScoreObject {
				stage = "ABC",
				score = 100,
				unit = "point" 
			});

			Assert.IsTrue(result);
		}

		[Test]
		public void GetScoreTestWithStage() {
			List<Objects.GBScoreObject> result = aClient.GetScore("ABC", "", 10, "");

			Console.Out.WriteLine ("GetScoreTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetTimeStamp() {
			Console.Out.WriteLine ("miliseconds: " + aClient.GetTimeStamp ());
			Console.Out.WriteLine ("daily: " + aClient.GetTimeStamp (Period.Daily));
			Console.Out.WriteLine ("weekly: " + aClient.GetTimeStamp (Period.Weekly, DayOfWeek.Sunday));

			Assert.LessOrEqual(ulong.Parse(aClient.GetTimeStamp (Period.Daily)), ulong.Parse(aClient.GetTimeStamp ()));
			Assert.LessOrEqual(ulong.Parse(aClient.GetTimeStamp (Period.Weekly, DayOfWeek.Sunday)), ulong.Parse(aClient.GetTimeStamp (Period.Daily)));
		}
	}
}
