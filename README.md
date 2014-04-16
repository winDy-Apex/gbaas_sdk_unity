GBaaS.io
==========

C# Client for the GBaaS.io : https://gbaas.io/

Client is in close Beta - only works with basic User functionality 
Results May Vary 

Usage is as follows :


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

더 자세한 내용은 GBaaS.io 사이트를 참조하세요.