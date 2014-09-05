using System;
using NUnit.Framework;
using GBaaS.io;
using GBaaS.io.Objects;
using System.Collections.Generic;

namespace GBaaS.io.Tests
{
	public class CustomOneObject : GBObject {
		public string mydataOne { get; set; }
		public string mydataTwo { get; set; }
		public string mydataThree { get; set; }
	}

	public class CollectionTests
	{
		[Test]
		public void GameDataSave()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			aClient.Login("test", "abc123");

			bool result = aClient.GameDataSave("key111", "GameSaveData----111");
			Assert.IsTrue(result);
		}

		[Test]
		public void GameDataLoad()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			aClient.Login("test", "abc123");

			string result = aClient.GameDataLoad("key111");
			Assert.IsTrue(result.CompareTo("GameSaveData----111") == 0);
		}


		[Test]
		public void GameDataLoadFail()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			aClient.Login("test", "abc123");

			string result = aClient.GameDataLoad("key111");
			Assert.IsFalse(result.CompareTo("GameSaveData----222") == 0);
		}

		[Test]
		public void CreateObject()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			CustomOneObject oneObject = new CustomOneObject {
				mydataOne = "One Data",
				mydataTwo = "Two Data",
				mydataThree = "Three Data"
			};

			bool result = oneObject.Save();
			Assert.IsTrue(result);
		}

		/*
		public string GetTypeNameTest<T>() {
			Console.Out.WriteLine(typeof(T).().ToString());
			return typeof(T).GetGenericArguments().ToString();
		}
		*/

		[Test]
		public void GetTypeNameTest() {
			string ret = GBCollectionService.Instance.GetTypeName(typeof(Objects.GBRoom));
			Assert.IsTrue(ret.Length > 0);
		}

		[Test]
		public void GetObject()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<CustomOneObject> collection = aClient2.GetObject<CustomOneObject>("mydataOne", "One Data", 1);
			Console.Out.WriteLine ("In GetObject Count : " + collection.Count.ToString());
			//Assert.IsTrue(collection.Count > 0);

			//var serializedParent = JsonConvert.SerializeObject(collection[0]); 
			//CustomOneObject customObject = JsonConvert.DeserializeObject<CustomOneObject>(collection[0].GetSerializedString());

			Assert.IsTrue(collection[0].mydataOne.CompareTo("One Data") == 0);
		}

		[Test]
		public void GetObjectAndSetLocation()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<CustomOneObject> collection = aClient2.GetObject<CustomOneObject>("type", "customoneobject");

			CustomOneObject customObject = collection[0];

			bool result = customObject.SetLocation(1.1f, 2.1f);

			Assert.IsTrue(result);
		}

		[Test]
		public void GetListInRange()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<Objects.GBObject> collection = aClient2.GetListInRange("CustomOneObject", 100, 1.1f, 2.1f);

			Assert.IsTrue(collection.Count > 0);
		}

		[Test]
		public void GetListInRangeOutBound()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<Objects.GBObject> collection = aClient2.GetListInRange("CustomOneObject", 100, 30.1f, 30.1f);

			Assert.IsTrue(collection.Count == 0);
		}

		[Test]
		public void GetList()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<Objects.GBObject> collection = aClient2.GetList("CustomOneObject");

			Assert.IsTrue(collection.Count > 0);
		}

		[Test]
		public void ModifyObject()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<CustomOneObject> collection = aClient2.GetObject<CustomOneObject>("mydataOne", "One Data");

			CustomOneObject customObject = collection[0];
			customObject.mydataThree = "Modify Three Data 12345777";
			bool result = customObject.Update();

			Assert.IsTrue(result);
		}

		[Test]
		public void DeleteObject()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<CustomOneObject> collection = aClient2.GetObject<CustomOneObject>("type", "customoneobject");

			/*
			var item = collection[0].GetJsonToken();

			CustomOneObject customObject = new CustomOneObject {
				mydataOne = (item["mydataOne"] ?? "").ToString(),
				mydataTwo = (item["mydataTwo"] ?? "").ToString(),
				mydataThree = (item["mydataThree"] ?? "").ToString()
			};
			*/

			CustomOneObject customObject = collection[0];

			//customObject.SetUUID((item["uuid"] ?? "").ToString());
			bool result = customObject.Delete();

			Assert.IsTrue(result);
		}

		[Test]
		public void CreateList()
		{
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			List<Objects.GBObject> collection = new List<Objects.GBObject>();

			for (int i = 0; i < 3; i++) {
				CustomOneObject oneObject = new CustomOneObject {
					mydataOne = i.ToString() + " One Data",
					mydataTwo = i.ToString() + " Two Data",
					mydataThree = i.ToString() + " Three Data"
				};

				collection.Add(oneObject);
			}

			bool result = aClient2.CreateList("CustomOneObject", collection);
			Assert.IsTrue(result);
		}

		[Ignore]
		[Test]
		public void GameDataSaveBigSizeValue()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL);
			aClient.Login("test", "abc123");

			string text = System.IO.File.ReadAllText(@"../../GameDataTestBigValue.txt"); // 2.2M size text File

			bool result = aClient.GameDataSave("keyBigSize", text);
			Assert.IsTrue(result);
		}

		[Test]
		public void ReceiptSave()
		{
			GBaaS.io.GBaaSApi aClient = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient.Login("test", "abc123");

			Objects.GBReceiptObject receipt = new Objects.GBReceiptObject {
				receiptCode = "12312313131213231123131231312313132132",
				receiptType = "1",
				userDID = "asjldjflajfdldsjlfjdlsajlfdsaFASFASF23234234243",
				dayToUse = "2014-04-16 15:28:32.0"
			};

			bool result = receipt.Save();
			Assert.IsTrue(result);

			result = aClient.ReceiptSave(receipt);
			Assert.IsTrue(result);
		}
			
		public class CustomUniqueObject : GBUniqueObject {
			public string temp { get; set; }
		}

		[Test]
		public void UniqueObjectCreate() {
			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login("test", "abc123");

			CustomUniqueObject uniqueObject = new CustomUniqueObject {
				temp = "Some5 Value"
			};
					
			bool result = uniqueObject.Save();
			Assert.IsTrue(result);

			CustomUniqueObject uniqueObject2 = new CustomUniqueObject {
				temp = "Some7 Value"
			};

			bool result2 = uniqueObject2.Save();
			Assert.IsTrue(result2);

			// Unique Object have to same uuid, It is same object on GBaaS, It's Unique.
			Assert.IsTrue(uniqueObject.uuid == uniqueObject2.uuid);

			// Unique Object must exist 0 or 1.
			Assert.IsTrue((uniqueObject._count <= 1 && uniqueObject2._count <= 1));
		}

		[Test]
		public void UniqueObjectUniquePerUserOnly() {
			GBaaS.io.GBaaSApi aClient1 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient1.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);

			CustomUniqueObject uniqueObject = new CustomUniqueObject {
				temp = "Some for test Value"
			};

			bool result = uniqueObject.Save();
			Assert.IsTrue(result);

			GBaaS.io.GBaaSApi aClient2 = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
			aClient2.Login(Defines.TEST_USERNAME1, Defines.TEST_PASSWORD);

			CustomUniqueObject uniqueObject2 = new CustomUniqueObject {
				temp = "Some for test1 Value"
			};

			bool result2 = uniqueObject2.Save();
			Assert.IsTrue(result2);

			// Unique Object have to different uuid per User.
			Assert.IsTrue(uniqueObject.uuid != uniqueObject2.uuid);
		}
	}
}
