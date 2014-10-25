using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using NUnit.Framework;

namespace GBaaS.io.Tests
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
}
