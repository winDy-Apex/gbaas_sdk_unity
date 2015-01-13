using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json.Linq;
using GBaaS.io.Utils;
using System.IO;

namespace GBaaS.io.Services
{
	/// <summary>
	/// 서버와 연결 정보를 가지고 있으며 서버 요청시 URL path 생성등을 담당한다.
	/// </summary>
	public class GBRequestService : GBService<GBRequestService>
	{
		private string _gbaasUrl { get; set; }

		public GBRequestService() {}

		public void SetGBaaSUrl(string gbaasUrl) {
			this._gbaasUrl = gbaasUrl;
		}

		public string GetToken(string username, string password) {
			var reqString = string.Format("/token/?grant_type=password&username={0}&password={1}", username, password);
			string rawResults;

			try {
				rawResults = HttpHelper.Instance.PerformGet(GBRequestService.Instance.BuildPath(reqString));
			} catch (Exception ex) {
				throw ex;
			}

			if (rawResults.Length > 0) {
				var results = JObject.Parse (rawResults);

				GBUserService.Instance.SetUserUUID(results["user"]["uuid"].ToString());

				return results ["access_token"].ToString ();
			} else {
				return "";
			}
		}

		public string LookUpToken(string token) {
			var reqString = "/users/me/?access_token=" + token;
			var rawResults = PerformRequest<string>(reqString);
			var entitiesResult = GetEntitiesFromJson(rawResults);

			return entitiesResult[0]["username"].ToString();
		}

		/// <summary>
		/// Performs a Get agianst the UserGridUrl + provided path
		/// </summary>
		/// <typeparam name="retrunT">Return Type</typeparam>
		/// <param name="path">Sub Path Of the Get Request</param>
		/// <returns>Object of Type T</returns>
		public retrunT PerformRequest<retrunT>(string path)
		{
			return PerformRequest<retrunT>(path, HttpHelper.RequestTypes.Get, null);
		}

		/// <summary>
		/// Performs a Request agianst the UserGridUrl + provided path
		/// </summary>
		/// <typeparam name="retrunT">Return Type</typeparam>
		/// <param name="path">Sub Path Of the Get Request</param>
		/// <returns>Object of Type T</returns>
		public retrunT PerformRequest<retrunT>(string path, HttpHelper.RequestTypes method, object data, string jsonParent = "")
		{
			string requestPath = BuildPath(path);
			try {
				return HttpHelper.Instance.PerformJsonRequest<retrunT>(requestPath, method, data, jsonParent);
			} catch (Exception ex) {
				throw ex;
			}
		}

		public bool PostUploadFile(string path, FileStream fileStream) {
			string requestPath = BuildPath(path);
			return HttpHelper.Instance.PostUploadFile(requestPath, fileStream);
		}

		public bool GetDownloadFile(string path, FileStream fileStream) {
			string requestPath = BuildPath(path);
			return HttpHelper.Instance.GetDownloadFile(requestPath, fileStream);
		}

		public JToken GetEntitiesFromJson(string rawJson) {
			if (string.IsNullOrEmpty(rawJson) != true) {
				JObject objResult = null;
				try {
					objResult = JObject.Parse(rawJson);
				} catch(Newtonsoft.Json.JsonReaderException e) {
					return null;
				}
				return objResult.SelectToken("entities");
			}
			return null;
		}

		// Get Cursor String (for continuous Query)
		public string GetValueFromJson(string key, string rawJson) {
			if (string.IsNullOrEmpty(rawJson) != true) {
				var objResult = JObject.Parse(rawJson);
				JToken token = objResult.SelectToken(key);
				if (token != null) {
					return token.ToString();
				}
			}
			return "";
		}

		/// <summary>
		/// Combines The UserGridUrl abd a provided path - checking to emsure proper http formatting
		/// </summary>
		/// <param name="path"></param>
		/// <returns></returns>
		private string BuildPath(string path)
		{
			StringBuilder sbResult = new StringBuilder();
			sbResult.Append(this._gbaasUrl);

			if (this._gbaasUrl.EndsWith("/") != true)
			{
				sbResult.Append("/");
			}

			if (path.StartsWith("/"))
			{
				path = path.TrimStart('/');
			}

			sbResult.Append(path);

			return sbResult.ToString();
		}
	}
}
