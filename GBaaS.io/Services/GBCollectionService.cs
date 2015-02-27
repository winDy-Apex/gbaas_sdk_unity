using System;
using GBaaS.io.Services;
using GBaaS.io.Objects;
using GBaaS.io.Utils;
using System.Threading;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.IO;
using Newtonsoft.Json;

namespace GBaaS.io
{
	public class GBCollectionService : GBService<GBCollectionService>
	{
		public GBCollectionService () {}

		public List<GBaaSApiHandler> _handler = new List<GBaaSApiHandler>();

		public void SetHandler(GBaaSApiHandler handler) {
			if (handler == null) {
				_handler.Clear();
			} else {
				_handler.Add(handler);
			}
		}

		private bool IsAsync() {
			return (_handler.Count > 0);
		}

		public List<GBAsset> GetFileList() {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetFileListThread());
				workerThread.Start();
				return null;
			} else {
				return this.GetFileListThread();
			}
		}

		public List<GBAsset> GetFileListThread() {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/assets", HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetFileList(default(List<GBAsset>));
					}
				} else {
					return default(List<GBAsset>);
				}
			}

			var assets = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetFileList(MakeList<GBAsset>(assets));
				}
			} else {
				return MakeList<GBAsset>(assets);
			}

			return default(List<GBAsset>);
		}

		public bool FileDownload(string fileName, FileStream fileStream) {
			/*
			var fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read,
				FileShare.ReadWrite | FileShare.Delete);
			*/

			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.FileDownloadThread(fileName, fileStream));
				workerThread.Start();
				return false;
			} else {
				return this.FileDownloadThread(fileName, fileStream);
			}
		}

		public bool FileDownloadThread(string fileName, FileStream fileStream) {
			// Check Asset Exist
			GBAsset asset = FindAsset(fileName);

			// Add New Asset
			if(asset == null) {
				return false;
			}

			// Download Asset Data
			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnFileDownload(DownloadAsset(asset.GetUUID(), fileStream));
				}
			} else {
				return DownloadAsset(asset.GetUUID(), fileStream);
			}

			return false;
		}

		private bool DownloadAsset(string assetUUID, FileStream fileStream) {
			return GBRequestService.Instance.GetDownloadFile("/assets/" + assetUUID + "/data", fileStream);
		}

		public bool FileUpload(string fileName, FileStream fileStream) {

			/*
			var fileStream = new FileStream(inputPath, FileMode.Open, FileAccess.Read,
				FileShare.ReadWrite | FileShare.Delete);
			*/

			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.FileUploadThread(fileName, fileStream));
				workerThread.Start();
				return false;
			} else {
				return this.FileUploadThread(fileName, fileStream);
			}
		}

		public bool FileUploadThread(string fileName, FileStream fileStream) {
			bool result = false;

			// Check Asset Exist
			GBAsset asset = FindAsset(fileName);

			// Add New Asset
			if(asset == null) {
				GBAsset addAsset = new GBAsset {
					owner = GBUserService.Instance.GetUserUUID(),
					path = fileName,
					name = fileName
				};

				result = addAsset.Save().isSuccess;

				if (result == true) {
					// Re Find Asset
					asset = FindAsset(fileName);
				}

				if (asset == null) {
					result = false;
				}

				if (result == false) {
					if (IsAsync()) {
						foreach (GBaaSApiHandler handler in _handler) {
							handler.OnFileUpload(result);
						}
					} else {
						return result;
					}
				}
			}

			// Upload Asset Data
			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGameDataSave(UploadAsset(asset.GetUUID(), fileStream));
				}
			} else {
				return UploadAsset(asset.GetUUID(), fileStream);
			}

			return false;
		}

		private bool UploadAsset(string assetUUID, FileStream fileStream) {
			return GBRequestService.Instance.PostUploadFile("/assets/" + assetUUID + "/data" , fileStream);
		}

		private GBAsset FindAsset(string key) {
			string query = "select *";
			query += " where";
			//query += " name = '" + "space-ship-373387_640.jpg" + "'";
			query += " name = '" + key + "'";

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/assets?ql=" + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				return null;
			}

			var assetData = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			/* Return Data Format
			[0]	{{   
					"uuid": "1e5fa6ea-6a5f-11e4-bfb3-49225183f47c",   
					"type": "asset",   
					"name": "space-ship-373387_640.jpg",   
					"created": 1415791749198,   
					"modified": 1415791750901,   
					"owner": "30e7d22a-3340-11e4-8952-792cb7addaa7",   
					"path": "icon_1415791749139",  
					"content-length": 166623,   
					"content-type": "image/jpeg",   
					"etag": "b3db9978afa2c4ffcfb813bd969d1839",  
					"metadata": 
						{     "path": "/assets/1e5fa6ea-6a5f-11e4-bfb3-49225183f47c"   } 
				}}	Newtonsoft.Json.Linq.JObject
			*/

			foreach (var item in assetData)
			{
				GBAsset obj = new GBAsset {
					owner = (item["owner"] ?? "").ToString(),
					path = (item["metadata"]["path"] ?? "").ToString()
				};

				obj.SetUUID((item["uuid"] ?? "").ToString());

				if (obj.path.Length > 0) {
					return obj;
				}
			}

			return null;
		}


		public bool GameDataSave(string key, string value) {

			value = AESEncrypt128(value, key);

			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GameDataSaveThread(key, value));
				workerThread.Start();
				return false;
			} else {
				return this.GameDataSaveThread(key, value);
			}
		}

		public bool GameDataSaveThread(string key, string value) {
			// if exist key, Value will Just Update.
			GBGameDataObject gameData = GetGameDataObject(key);

			// if don't exist key, GameData Object will Create.
			if (gameData == null) {
				gameData = new GBGameDataObject {
					username = GBUserService.Instance.GetLoginName(),
					key = key,
					value = value
				};

				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGameDataSave(gameData.Save().isSuccess);
					}
				} else {
					return gameData.Save().isSuccess;
				}
			} else {
				gameData.value = value;
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGameDataSave(gameData.Update().isSuccess);
					}
				} else {
					return gameData.Update().isSuccess;
				}
			}

			return false;
		}

		private GBGameDataObject GetGameDataObject(string key) {
			string query = "select *";

			query += " where";
			query += " username = '" + GBUserService.Instance.GetLoginName() + "'";
			query += " and key = '" + key + "'";
			query += " order by modified desc";
			query += " limit 1";

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/GameData?ql=" + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				return null;
			}

			var gameData = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			string result = "";

			foreach (var item in gameData)
			{
				Objects.GBGameDataObject obj = new Objects.GBGameDataObject {
					username = (item["username"] ?? "").ToString(),
					key = (item["key"] ?? "").ToString(),
					value = (item["value"] ?? "").ToString()
				};

				obj.SetUUID((item["uuid"] ?? "").ToString());

				result = obj.value;
				if (result.Length > 0) {
					return obj;
				}
			}

			return null;
		}

		public string GameDataLoad(string key) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GameDataLoadThread(key));
				workerThread.Start();
				return default(string);
			} else {
				return this.GameDataLoadThread(key);
			}
		}

		private string GameDataLoadThread(string key) {
			string result = "";

			Objects.GBGameDataObject obj = GetGameDataObject(key);
			if (obj != null) {
				result = AESDecrypt128(obj.value, obj.key);
			}

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGameDataLoad(result);
				}
			} else {
				return result;
			}

			return default(string);
		}

		//AES_128 암호화
		public string AESEncrypt128(string Input, string key)
		{

			RijndaelManaged RijndaelCipher = new RijndaelManaged();

			byte[] PlainText = System.Text.Encoding.Unicode.GetBytes(Input);
			byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());

			PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
			ICryptoTransform Encryptor = RijndaelCipher.CreateEncryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));

			MemoryStream memoryStream = new MemoryStream();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, Encryptor, CryptoStreamMode.Write);

			cryptoStream.Write(PlainText, 0, PlainText.Length);
			cryptoStream.FlushFinalBlock();

			byte[] CipherBytes = memoryStream.ToArray();

			memoryStream.Close();
			cryptoStream.Close();

			string EncryptedData = Convert.ToBase64String(CipherBytes);

			return EncryptedData;
		}

		//AE_S128 복호화
		public string AESDecrypt128(string Input, string key)
		{
			RijndaelManaged RijndaelCipher = new RijndaelManaged();

			byte[] EncryptedData = Convert.FromBase64String(Input);
			byte[] Salt = Encoding.ASCII.GetBytes(key.Length.ToString());

			PasswordDeriveBytes SecretKey = new PasswordDeriveBytes(key, Salt);
			ICryptoTransform Decryptor = RijndaelCipher.CreateDecryptor(SecretKey.GetBytes(32), SecretKey.GetBytes(16));
			MemoryStream memoryStream = new MemoryStream(EncryptedData);
			CryptoStream cryptoStream = new CryptoStream(memoryStream, Decryptor, CryptoStreamMode.Read);

			byte[] PlainText = new byte[EncryptedData.Length];

			int DecryptedCount = cryptoStream.Read(PlainText, 0, PlainText.Length);

			memoryStream.Close();
			cryptoStream.Close();

			string DecryptedData = Encoding.Unicode.GetString(PlainText, 0, DecryptedCount);

			return DecryptedData;
		}

		public bool CreateList(string collectionName, List<Objects.GBObject> list) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.CreateListThread(collectionName, list));
				workerThread.Start();
				return false;
			} else {
				return this.CreateListThread(collectionName, list);
			}
		}

		public bool CreateListThread(string collectionName, List<Objects.GBObject> list) {
			bool result = false;		
			foreach (var item in list) {
				result = item.Save().isSuccess;
				if (result == false)
					break;
			}

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnCreateList(result);
				}
			} else {
				return result;
			}

			return false;
		}

		public List<Objects.GBObject> GetList(string collectionName) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetListThread(collectionName));
				workerThread.Start();
				return default(List<Objects.GBObject>);
			} else {
				return this.GetListThread(collectionName);
			}
		}

		private List<Objects.GBObject> GetListThread(string collectionName) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + collectionName);
			var collection = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetList(MakeList<Objects.GBObject>(collection));
				}
			} else {
				return MakeList<Objects.GBObject>(collection);
			}

			return default(List<Objects.GBObject>);
		}

		public List<Objects.GBObject> GetListByKey(string collectionName, string key, string value) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetListByKeyThread(collectionName, key, value));
				workerThread.Start();
				return default(List<Objects.GBObject>);
			} else {
				return this.GetListByKeyThread(collectionName, key, value);
			}
		}

		private List<Objects.GBObject> GetListByKeyThread(string collectionName, string key, string value) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + collectionName + "?ql=select * where " + key + "='" + value + "'");
			var collection = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetList(MakeList<Objects.GBObject>(collection));
				}
			} else {
				return MakeList<Objects.GBObject>(collection);
			}

			return default(List<Objects.GBObject>);
		}

		public List<Objects.GBObject> GetListInRange(string collectionName, float meters, float latitude, float longitude) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetListInRangeThread(collectionName, meters, latitude, longitude));
				workerThread.Start();
				return default(List<Objects.GBObject>);
			} else {
				return this.GetListInRangeThread(collectionName, meters, latitude, longitude);
			}
		}

		private List<Objects.GBObject> GetListInRangeThread(string collectionName, float meters, float latitude, float longitude) {
			string query = "?ql=\"location within " + meters.ToString() + " of " + latitude.ToString() + "," + longitude.ToString() + "\"";
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + collectionName + query);
			var collection = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetList(MakeList<Objects.GBObject>(collection));
				}
			} else {
				return MakeList<Objects.GBObject>(collection);
			}

			return default(List<Objects.GBObject>);
		}

		public List<T> GetObject<T>(string key, string value, int limit = 1) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetObjectThread<T>(key, value, limit));
				workerThread.Start();
				return default(List<T>);
			} else {
				return this.GetObjectThread<T>(key, value, limit);
			}
		}

		private List<T> GetObjectThread<T>(string key, string value, int limit) {
			string query = "";

			string baseTypeName = typeof(T).BaseType.Name;
			string basequery = "";

			if (baseTypeName.CompareTo("GBUniqueObject") == 0) {
				basequery = " username = '" + GBUserService.Instance.GetLoginName() + "'";
			}

			if ((basequery.Length + key.Length + value.Length) > 0) {
				query = "?ql=select * where";

				if (basequery.Length > 0) {
					query += basequery;
				}

				if(basequery.Length > 0 && key.Length > 0) {
					query += " and";
				}

				if (key.Length > 0) {
					query += " " + key + " = '" + value + "' order by " + key + " desc";
				}

				if (limit > 0) {
					query += " limit " + limit.ToString();
				}
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetTypeName(typeof(T)) + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetObject(default(List<T>));
					}
				} else {
					return default(List<T>);
				}
			}

			var collection = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetObject<T>(MakeList<T>(collection));
				}
			} else {
				return MakeList<T>(collection);
			}

			return default(List<T>);
		}


		public List<T> GetObjectByName<T>(string objectName, string key, string value, int limit = 1) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetObjectByNameThread<T>(objectName, key, value, limit));
				workerThread.Start();
				return default(List<T>);
			} else {
				return this.GetObjectByNameThread<T>(objectName, key, value, limit);
			}
		}

		private List<T> GetObjectByNameThread<T>(string objectName, string key, string value, int limit) {
			string query = "";

			if (key.Length > 0) {
				query = "?ql=select *";

				if ((key.Length + value.Length) > 0) {
					query += " where";
				}

				if (key.Length > 0) {
					query += " " + key + " = '" + value + "'";
				}

				query += " order by " + key + " desc";

				if (limit > 0) {
					query += " limit " + limit.ToString();
				}
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + objectName + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetObject(default(List<T>));
					}
				} else {
					return default(List<T>);
				}
			}

			var collection = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetObject<T>(MakeList<T>(collection));
				}
			} else {
				return MakeList<T>(collection);
			}

			return default(List<T>);
		}

		private List<T> MakeList<T>(Newtonsoft.Json.Linq.JToken collection) {
			List<T> results = new List<T>();
			foreach (var item in collection) {
				results.Add(JsonConvert.DeserializeObject<T>(item.ToString()));
			}

			return results;
		}

		public string GetTypeName(Type t) {
			if (!t.IsGenericType) return t.Name;
			if (t.IsNested && t.DeclaringType.IsGenericType) throw new NotImplementedException();
			string txt = t.Name.Substring(0, t.Name.IndexOf('`')) + "<";
			int cnt = 0;
			foreach (Type arg in t.GetGenericArguments()) {
				if (cnt > 0) txt += ", ";
				txt += GetTypeName(arg);
				cnt++;
			}
			return txt + ">";
		}
	}
}
