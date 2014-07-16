using System;
using NUnit.Framework;
using GBaaS.io;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using GBaaS.io.Utils;
using System.Collections.Generic;

namespace GBaaS.io.Tests
{
	public class CustomApiCallParams {
		public string app { get; set; }
	}

	public class CustomApiTests
	{
		[Test]
		public void CallCustomApi()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient.Login("test", "abc123");

			string token = HttpHelper.Instance._accessToken;
			Assert.IsNotEmpty(token);

			// CustomoApi Must Have json params.
			CustomApiCallParams param = new CustomApiCallParams();
			param.app = "gbaas";

			//var rawResults = GBRequestService.Instance.PerformRequest<string>("/customapis/function/hello");
			var rawResults = HttpHelper.Instance.PerformJsonRequest<string>(
				"https://api.gbaas.io/de5d2505c8d34410c9d8966e1e8fac11/gbaasman2/customapi/function/GetFormatData"
				, HttpHelper.RequestTypes.Post, param, "");
			var entitiesResult = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			if (entitiesResult != null) {
			} else {
			}

			Assert.IsNotNull(rawResults);
		}
	}
}
