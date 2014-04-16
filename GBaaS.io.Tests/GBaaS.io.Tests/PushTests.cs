using System;
using NUnit.Framework;
using GBaaS.io;

namespace GBaaS.io.Tests
{
	public class PushTests
	{
		[Test]
		public void SendMessage()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			bool result = aClient2.SendMessage("TestMessage", "", "", "", "", PushSendType.alldevices, PushScheduleType.now);
			Assert.IsTrue(result);
		}

		[Test]
		public void DeviceRegistration()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			bool result = aClient2.RegisterDevice("iPhone", "7.1", "iOS", "1234-5678-9000");
			Assert.IsTrue(result);
		}
	}
}
