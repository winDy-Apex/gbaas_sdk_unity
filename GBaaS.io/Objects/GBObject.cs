using System;
using GBaaS.io.Services;
using GBaaS.io.Utils;

namespace GBaaS.io.Objects {
	public class GBObject : GBResult {
		public string uuid { get; set; }

		private 	string _entityType 	= "";
		protected 	Newtonsoft.Json.Linq.JToken _jsonToken;

		public GBObject () {}

		public void SetUUID(string init_uuid) {
			uuid = init_uuid;
		}

		public string GetUUID() {
			return uuid;
		}

		public void SetJsonToken(Newtonsoft.Json.Linq.JToken jsonToken) {
			_jsonToken = jsonToken;
		}

		public Newtonsoft.Json.Linq.JToken GetJsonToken() {
			return _jsonToken;
		}

		public string GetSerializedString() {
			return _jsonToken.ToString();
		}

		public virtual GBResult Save() { 
			string rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType(), HttpHelper.RequestTypes.Post, this);
			bool success = HttpHelper.Instance.CheckSuccess(rawResults);
			if (isSuccess) {
				return new GBResult { isSuccess = success, returnCode = ReturnCode.Success, reason = rawResults };
			}
			return new GBResult { isSuccess = success, returnCode = ReturnCode.FailWithReason, reason = rawResults };
		}

		public GBResult Update() { 
			if (uuid == null) {
				return Save();
			}
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + uuid, HttpHelper.RequestTypes.Put, this);

			bool success = HttpHelper.Instance.CheckSuccess(rawResults);
			if (isSuccess) {
				return new GBResult { isSuccess = success, returnCode = ReturnCode.Success, reason = rawResults };
			}
			return new GBResult { isSuccess = success, returnCode = ReturnCode.FailWithReason, reason = rawResults };
		}

		public bool Delete() { 
			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + uuid, HttpHelper.RequestTypes.Delete, null);
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}

		public virtual bool Load() { return false; }

		public void SetEntityType(string entityType) {
			_entityType = entityType;
		}

		public String GetEntityType() {
			if (_entityType.Length > 0) {
				return _entityType.ToLower();
			}

			try {
				string 	s 				= this.GetType().Name;
				string 	prefixString	= "GB";
				if(s.IndexOf(prefixString) < 0) {
					return s.ToLower(); // GB로 시작하지 않으면(Custom Object) 이름 그대로를 리턴한다.
				}
				int 	prefix 			= s.IndexOf(prefixString) + prefixString.Length;
				int 	postfix 		= s.IndexOf("Object");

				return s.Substring(prefix, postfix - prefix).ToLower();
			} catch {
				return "";
			}
		}

		public bool SetLocation(float latitude, float longitude) {
			GBLocationObject location = new GBLocationObject {
				latitude = latitude,
				longitude = longitude
			};

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + GetEntityType() + "/" + GetUUID(), HttpHelper.RequestTypes.Put, location, "location");
			return HttpHelper.Instance.CheckSuccess(rawResults);
		}
	}
}
