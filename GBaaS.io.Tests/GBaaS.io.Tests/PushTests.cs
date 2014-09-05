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

			bool result = aClient2.RegisterDevice("iPhone", "7.1", "iOS", "1234-5678-9000111");

			/*
			bool result = aClient2.RegisterDevice("PANTECH_IM-A890L", 
				"Android_OS_4.2.2_/_API-17_(JDQ39/IM-A890L.008)", 
				"android", 
				"APA91bGgQ-isAC8gPmTHodPByeUB5wjOy4qjagFiyxPFmOH1q-JvkanoW1UeuzypULRi1Z16TIrDmVJqdQNA4BCRDDuL6IyedUQY8XftI7dzhCnLPJ1rT75hELB0sZvdXEd7COWHcHGJumHUxRO7baSXEC6WM4f8bQ");
			*/

			Assert.IsTrue(result);
		}

		[Test]
		public void IsRegisteredDevice()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			bool result = aClient2.IsRegisteredDevice("iPhone", "7.1", "iOS", "1234-5678-9000111");

			/*
			bool result = aClient2.RegisterDevice("PANTECH_IM-A890L", 
				"Android_OS_4.2.2_/_API-17_(JDQ39/IM-A890L.008)", 
				"android", 
				"APA91bGgQ-isAC8gPmTHodPByeUB5wjOy4qjagFiyxPFmOH1q-JvkanoW1UeuzypULRi1Z16TIrDmVJqdQNA4BCRDDuL6IyedUQY8XftI7dzhCnLPJ1rT75hELB0sZvdXEd7COWHcHGJumHUxRO7baSXEC6WM4f8bQ");
			*/

			Assert.IsTrue(result);
		}
	}
}
