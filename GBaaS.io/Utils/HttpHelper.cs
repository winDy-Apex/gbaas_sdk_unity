using Krystalware.UploadHelper;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace GBaaS.io.Utils
{
	public class HttpHelper : Singleton<HttpHelper>
	{
		public const int TIMEOUT = 3000;

		public enum RequestTypes { Get, Post, Put, Delete }
		
		public string _accessToken { get; set; }

		#region Get
		public string PerformGet(string url)
		{
			try {
				WebRequest req = WebRequest.Create(url);

				if (_accessToken != null && _accessToken.Length > 0) {
					req.Headers.Add ("Authorization: Bearer " + _accessToken);
				}

				req.Timeout=TIMEOUT;
				using(WebResponse resp = req.GetResponse()) {
					StreamReader sr = new StreamReader(resp.GetResponseStream());
					return sr.ReadToEnd().Trim();
				}
			} catch (WebException e) { // HTTP Call Fail or No Result Set
				e.ToString();
			}

			return "";
		}
		#endregion

		#region Post

		public ReturnT PerformPost<ReturnT>(string url)
		{
			return PerformPost<object, ReturnT>(url, new object());
		}

		public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData)
		{
			return PerformPost<PostT, ReturnT>(url, postData, new NameValueCollection());
		}

		public ReturnT PerformPost<PostT, ReturnT>(string url, PostT postData, NameValueCollection files)
		{
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Timeout=TIMEOUT;

			NameValueCollection nvpPost;
			if (typeof(PostT) == typeof(NameValueCollection))
			{
				nvpPost = postData as NameValueCollection;
			}
			else
			{
				nvpPost = ObjectToNameValueCollection<PostT>(postData);
			}

			List<UploadFile> postFiles = new List<UploadFile>();
			foreach (var fKey in files.AllKeys)
			{
				FileStream fs = File.OpenRead(files[fKey]);
				postFiles.Add(new UploadFile(fs, fKey, files[fKey], "application/octet-stream"));
			}
				
			try
			{
				var response = HttpUploadHelper.Upload(req, postFiles.ToArray(), nvpPost);

				using (Stream s = response.GetResponseStream())
					using (StreamReader sr = new StreamReader(s))
				{
					var responseJson = sr.ReadToEnd();
					if (typeof(ReturnT) == typeof(string))
					{
						return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
					}

					return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
				}
			}
			catch (WebException ex)
			{
				ex.ToString();
				return default(ReturnT);
			}
		}

		//Converts an object to a name value collection (for posts)
		private NameValueCollection ObjectToNameValueCollection<T>(T obj)
		{
			NameValueCollection results = new NameValueCollection();

			var oType = typeof(T);
			foreach (var prop in oType.GetProperties())
			{
				string pVal = "";
				try
				{
					pVal = oType.GetProperty(prop.Name).GetValue(obj, null).ToString();
				}
				catch { }
				results[prop.Name] = pVal;
			}

			return results;
		}

		#endregion

		#region JSON Request

		public ReturnT PerformJsonRequest<ReturnT>(string url, RequestTypes method, object postData, string jsonParent = "") {
			//Initilize the http request
			HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
			req.Timeout=TIMEOUT;
			req.ContentType = "application/json";
			req.Method = Enum.GetName(typeof(RequestTypes), method).ToUpper();
			if (_accessToken != null && _accessToken.Length > 0) {
				req.Headers.Add ("Authorization: Bearer " + _accessToken);
			}

			//If posting data - serialize it to a json object
			if (method != RequestTypes.Get && postData != null) {
				StringBuilder sbJsonRequest = new StringBuilder();
				var T = postData.GetType();
				foreach (var prop in T.GetProperties()) {
					if (NativeTypes.Contains(prop.PropertyType) && prop.GetValue(postData, null) != null) {
						//Console.Out.WriteLine(prop.PropertyType.ToString());
						if (prop.PropertyType.ToString().CompareTo("System.Boolean") == 0) {
							object value = prop.GetValue(postData, null);
							String valueString = "";
							if (value != null) {
								valueString = value.ToString().ToLower();
							}

							sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name, valueString);
						} else if (prop.PropertyType.ToString().CompareTo("System.Int32") == 0 || prop.PropertyType.ToString().CompareTo("System.Single") == 0) {
							//sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name.ToLower(), prop.GetValue(postData, null));
							sbJsonRequest.AppendFormat("\"{0}\":{1},", prop.Name, prop.GetValue(postData, null));
						} else {
							//sbJsonRequest.AppendFormat("\"{0}\":\"{1}\",", prop.Name.ToLower(), prop.GetValue(postData, null));
							sbJsonRequest.AppendFormat("\"{0}\":\"{1}\",", prop.Name, prop.GetValue(postData, null));
						}
					}
				}

				using (var sWriter = new StreamWriter(req.GetRequestStream())) {
					if (jsonParent.Length > 0) {
						sWriter.Write("{\"" + jsonParent + "\": {" + sbJsonRequest.ToString().TrimEnd(',') + "} }");
					} else {
						sWriter.Write("{" + sbJsonRequest.ToString().TrimEnd(',') + "}");
					}
				}
			}

			//Submit the Http Request
			string responseJson = "";
			try {
				using (var wResponse = req.GetResponse()) {
					StreamReader sReader = new StreamReader(wResponse.GetResponseStream());
					responseJson = sReader.ReadToEnd();
				}
			} catch (WebException ex) {
				using (WebResponse response = ex.Response) {
					if (response != null) {
						StreamReader sReader = new StreamReader(response.GetResponseStream());
						responseJson = sReader.ReadToEnd();
					} else {
						return default(ReturnT);
					}
				}
			}

			if (typeof(ReturnT) == typeof(string)) {
				return (ReturnT)Convert.ChangeType(responseJson, typeof(ReturnT));
			}

			return fastJSON.JSON.Instance.ToObject<ReturnT>(responseJson);
		}

		public bool GetDownloadFile(string url, FileStream fileStream) {
			WebClient webClient = new WebClient();
			byte[] result = webClient.DownloadData(url);

			fileStream.Write(result, 0, result.Length);

			return true;
		}

		public bool PostUploadFile(string url, FileStream fileStream) {
			if (fileStream == null) {
				return false;
			}

			HttpWebRequest httpWebRequest 	= (HttpWebRequest)WebRequest.Create(url);
			httpWebRequest.ContentType 		= "application/octet-stream";
			httpWebRequest.Method 			= "POST";
			httpWebRequest.KeepAlive 		= true;
			httpWebRequest.Credentials 		= System.Net.CredentialCache.DefaultCredentials;

			if (_accessToken != null && _accessToken.Length > 0) {
				httpWebRequest.Headers.Add ("Authorization: Bearer " + _accessToken);
			}

			Stream 	memStream 	= new System.IO.MemoryStream();
			byte[] 	buffer 		= new byte[1024];
			int 	bytesRead 	= 0;
			while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0) {
				memStream.Write(buffer, 0, bytesRead);
			}

			fileStream.Close();

			httpWebRequest.ContentLength = memStream.Length;
			Stream requestStream = httpWebRequest.GetRequestStream();
			memStream.Position = 0;
			byte[] tempBuffer = new byte[memStream.Length];
			memStream.Read(tempBuffer, 0, tempBuffer.Length);
			memStream.Close();
			requestStream.Write(tempBuffer, 0, tempBuffer.Length);
			requestStream.Close();
			try
			{
				WebResponse webResponse = httpWebRequest.GetResponse();
				Stream stream = webResponse.GetResponseStream();
				StreamReader reader = new StreamReader(stream);
				string var = reader.ReadToEnd();

			}

			catch (Exception ex)
			{
				//response.InnerHtml = ex.Message;
				return false;
			}
			httpWebRequest = null;
			return true;
		}

		//Approved Types for serialization
		public List<Type> NativeTypes
		{
			get
			{
				var approvedTypes = new List<Type>();

				approvedTypes.Add(typeof(int));
				approvedTypes.Add(typeof(Int32));
				approvedTypes.Add(typeof(Int64));
				approvedTypes.Add(typeof(string));
				approvedTypes.Add(typeof(DateTime));
				approvedTypes.Add(typeof(double));
				approvedTypes.Add(typeof(decimal));
				approvedTypes.Add(typeof(float));
				approvedTypes.Add(typeof(List<>));
				approvedTypes.Add(typeof(bool));
				approvedTypes.Add(typeof(Boolean));

				approvedTypes.Add(typeof(int?));
				approvedTypes.Add(typeof(Int32?));
				approvedTypes.Add(typeof(Int64?));
				approvedTypes.Add(typeof(DateTime?));
				approvedTypes.Add(typeof(double?));
				approvedTypes.Add(typeof(decimal?));
				approvedTypes.Add(typeof(float?));
				approvedTypes.Add(typeof(bool?));
				approvedTypes.Add(typeof(Boolean?));

				return approvedTypes;
			}
		}

		#endregion

		public bool CheckSuccess(string result) {
			return (result.IndexOf ("error") == -1);
		}

		public bool SafeConvertBoolean(string booleanString) {
			return booleanString.ToLower().Equals("true");
		}
	}
}
