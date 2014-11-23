using System;
using NUnit.Framework;
using GBaaS.io;
using System.IO;
using System.Collections.Generic;
using GBaaS.io.Objects;

namespace GBaaS.io.Tests
{
	public class FileStoreTests
	{
		//GBaaS.io.GBaaSApi _gbaas = new GBaaS.io.GBaaSApi(Defines.USERGRID_TEST_URL);
		GBaaS.io.GBaaSApi _gbaas = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL2);
		//string _token = "";

		[Test]
		public void FileUploadTest()
		{
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			string currentPath = Directory.GetCurrentDirectory() + "/../../";
			string fileName = "test.jpg";
			FileStream fileStream = new FileStream(currentPath + fileName, FileMode.Open, FileAccess.Read);

			bool result = _gbaas.FileUpload(fileName, fileStream);

			fileStream.Close();
			Assert.IsTrue(result);
		}

		[Test]
		public void FileDownloadTest()
		{
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			string currentPath 		= Directory.GetCurrentDirectory() + "/../../";
			string fileName 		= "test.jpg";
			string downloadFileName	= "test_download.jpg";

			FileStream fileStream = new FileStream(currentPath + downloadFileName, FileMode.Create, FileAccess.Write);

			bool result = _gbaas.FileDownload(fileName, fileStream);

			fileStream.Close();
			Assert.IsTrue(result);
		}

		[Test]
		public void GetFileList() {
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<GBAsset> fileList = _gbaas.GetFileList();

			Assert.IsNotNull(fileList);
		}
	}
}