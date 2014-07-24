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
						handler.OnGameDataSave(gameData.Save());
					}
				} else {
					return gameData.Save();
				}
			} else {
				gameData.value = value;
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGameDataSave(gameData.Update());
					}
				} else {
					return gameData.Update();
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
				result = item.Save();
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
			string query = "select *";

			if ((key.Length + value.Length) > 0) {
				query += " where";
			}

			if (key.Length > 0) {
				query += " " + key + " = '" + value + "'";
			}
		
			query += " order by " + key + " desc";

			if (limit > 0) {
				query += " limit " + limit.ToString ();
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetTypeName(typeof(T)) + "?ql=" + query, HttpHelper.RequestTypes.Get, "");
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
