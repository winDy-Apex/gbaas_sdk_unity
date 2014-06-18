using System;
using NUnit.Framework;
using GBaaS.io;
using System.Collections.Generic;
using GBaaS.io.Objects;

namespace GBaaS.io.Tests.Net
{
	public class UserHandler : GBaaSApiHandler {
		public override void OnReceiveData(string recvPacket) {
			Console.Out.WriteLine("On ReceiveData : " + recvPacket);

			//if (recvPacket.IndexOf("\"PacketType\" : 14") > 0) {
			//	return;
			//}

			AsyncCallChecker.Instance.SetAsyncCalling(false);

			Assert.IsNotNull(recvPacket);
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

	[Ignore]
	public class NetTests
	{
		[Test]
		public void ConnectTest()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			Boolean result = aClient2.ConnectNetService("14.63.196.35", 31499);
			Assert.IsTrue(result);
		}

		[Test]
		public void SessionIn()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			Boolean result = aClient2.ConnectNetService("14.63.196.35", 31499);
			Assert.IsTrue(result);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			result = aClient2.SessionIn("test", "AAA");

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			Assert.IsTrue(result);
		}

		[Test]
		public void GetRoomList()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.LobbyIn("14.63.196.35", 31499, "AAA");

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			aClient2.GetRoomList();

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			Assert.IsNotNull(result);
		}

		[Test]
		public void SendContinuedPacket()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);

			GBaaSApiHandler handler = new UserHandler();
			aClient2.AddHandler(handler);

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			var result = aClient2.LobbyIn("127.0.0.1", 31499, "AAA");

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			AsyncCallChecker.Instance.SetAsyncCalling(true);

			aClient2.ChannelSend("Test Continued Packet1", 0, true);
			result = aClient2.ChannelSend("Test Continued Packet2", 0, true);

			//Async 호출이 끝날때까지 대기
			while (AsyncCallChecker.Instance.GetAsyncCalling()) {
				Console.Out.WriteLine ("...AsyncCalling...");
				System.Threading.Thread.Sleep(100);
			}

			Assert.IsNotNull(result);
		}
	}
}
