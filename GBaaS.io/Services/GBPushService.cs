using System;
using GBaaS.io.Services;
using GBaaS.io.Objects;
using GBaaS.io.Utils;
using System.Threading;
using System.Collections.Generic;

namespace GBaaS.io
{
	class GBPushService : GBService<GBPushService>
	{
		public GBPushService () {}

		public List<GBaaSApiHandler> _handler = new List<GBaaSApiHandler>();

		public void SetHandler(GBaaSApiHandler handler) {
			if (handler == null) {
				_handler.Clear();
			} else {
				_handler.Add(handler);
			}
		}

		private bool IsAsync(bool forceSync = false) {
			if (forceSync)
				return false;
			return (_handler.Count > 0);
		}

		public bool SendMessage(
			string message, string scheduleDate, string deviceIds, string groupPaths, string userNames, 
			PushSendType sendType, PushScheduleType scheduleType) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.SendMessageThread(message, scheduleDate, deviceIds, groupPaths, userNames, sendType, scheduleType));
				workerThread.Start();
				return false;
			} else {
				return this.SendMessageThread(message, scheduleDate, deviceIds, groupPaths, userNames, sendType, scheduleType);
			}
		}

		private bool SendMessageThread(
			string message, string scheduleDate, string deviceIds, string groupPaths, string userNames, 
			PushSendType sendType, PushScheduleType scheduleType) {
			GBPushMessageObject pushMessage = new GBPushMessageObject {
				message = message,
				scheduleDate = scheduleDate,
				deviceIds = deviceIds,
				groupPaths = groupPaths,
				userNames = userNames,
				sendType = sendType.ToString(),
				scheduleType = scheduleType.ToString()
			};

			bool result = false;

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnSendMessage(pushMessage.Save());
				}
			} else {
				result = pushMessage.Save();
			}

			return result;
		}

		public bool RegisterDevice(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registration_id, bool forceSync) {
			if (IsAsync(forceSync)) {
				Thread workerThread = new Thread(() => this.RegisterDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registration_id, forceSync));
				workerThread.Start();
				return false;
			} else {
				return this.RegisterDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registration_id, forceSync);
			}
		}

		private bool RegisterDeviceThread(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registration_id, bool forceSync) {

			deviceModel = deviceModel.Replace(" ", "_");
			deviceOSVersion = deviceOSVersion.Replace(" ", "_");


			bool isRegistered = IsRegisteredDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registration_id, true);

			if (isRegistered) {
				try {
					if (IsAsync(forceSync)) {
						foreach (GBaaSApiHandler handler in _handler) {
							handler.OnRegisterDevice(isRegistered);
						}
					}
				} catch(Exception e) {

				}

				return isRegistered;
			} else {
				// 등록안된 디바이스인경우 등록
				GBDeviceRegisterObject deviceRegister = new GBDeviceRegisterObject {
					deviceModel = deviceModel,
					deviceOSVersion = deviceOSVersion,
					devicePlatform = devicePlatform,
					registration_id = registration_id
				};

				bool result = deviceRegister.Save();
				if (result == false) {
					if (IsAsync(forceSync)) {
						foreach (GBaaSApiHandler handler in _handler) {
							handler.OnRegisterDevice(result);
						}
					}

					return result;
				}

				// 등록된 디바이스 UUID 정보 가져오기
				//rawResults = GBRequestService.Instance.PerformRequest<string>("/devices?registration_id=" + registration_id, HttpHelper.RequestTypes.Get, "");
				var rawResults = GBRequestService.Instance.PerformRequest<string>(
					"/devices?ql=select%20*%20where%20registration_id='" + registration_id + "'", HttpHelper.RequestTypes.Get, "");

				isRegistered = HttpHelper.Instance.CheckSuccess(rawResults);

				string deviceID = "";

				if (isRegistered) {
					Newtonsoft.Json.Linq.JToken registeredDevices = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
					foreach (var device in registeredDevices) {
						deviceID = (device["uuid"] ?? "").ToString();
						if (deviceID.Length > 0)
							break;
					}
				}

				if (IsAsync(forceSync)) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnRegisterDevice(ConnectDeviceToUser(deviceID));
					}
				} else {
					return ConnectDeviceToUser(deviceID);
				}
			}

			return false;
		}

		public bool IsRegisteredDevice(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registration_id) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.IsRegisteredDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registration_id));
				workerThread.Start();
				return false;
			} else {
				return this.IsRegisteredDeviceThread(deviceModel, deviceOSVersion, devicePlatform, registration_id);
			}
		}

		private bool IsRegisteredDeviceThread(
			string deviceModel, string deviceOSVersion, string devicePlatform, string registration_id, bool forceSync = false) {

			deviceModel = deviceModel.Replace(" ", "_");
			deviceOSVersion = deviceOSVersion.Replace(" ", "_");

			// 등록된 디바이스인지 확인
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/devices?ql=select%20*%20where%20registration_id='" + registration_id + "'", HttpHelper.RequestTypes.Get, "");

			bool isRegistered = true;
			Newtonsoft.Json.Linq.JToken registeredDevices = null;

			if (HttpHelper.Instance.CheckSuccess(rawResults)) {
				registeredDevices = GBRequestService.Instance.GetEntitiesFromJson(rawResults);
				if (registeredDevices.HasValues == false) {
					isRegistered = false;
				}
			}

			try {
				if (IsAsync() && forceSync == false) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnIsRegisteredDevice(isRegistered);
					}
				}
			} catch(Exception e) {

			}

			return isRegistered;
		}

		private bool ConnectDeviceToUser(string deviceID) {
			var rawResults = GBRequestService.Instance.PerformRequest<string>(
				"/users/" + GBUserService.Instance.GetLoginName() + "/devices/" + deviceID, HttpHelper.RequestTypes.Post, "");

			return HttpHelper.Instance.CheckSuccess(rawResults);
		}
	}
}
