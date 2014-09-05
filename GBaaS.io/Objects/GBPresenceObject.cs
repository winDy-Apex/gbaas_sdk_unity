using System;
using System.Collections.Generic;

namespace GBaaS.io.Objects
{
	public class GBPresenceObject : GBUniqueObject
	{
		public string lastPresenceDate { get; set; }
		public int monthlyPresence { get; set; }
		public int lastMonthlyPresence { get; set; }

		// 하루에 한번 monthlyPresence 를 증가 시킨다.
		// date parameter 는 인위적으로 마지막 로그인 날짜를 조작하기 위해서만 사용한다. (테스트 등)
		public bool DoPresence(string date = "") {
			PresenceLoad();

			if (date.Length > 0) {
				lastPresenceDate = date;
			}

			DateTime lastDT = Convert.ToDateTime(lastPresenceDate);
			DateTime nowDT = DateTime.Now;

			if (lastDT.Date != nowDT.Date) {

				if (lastDT.Month != nowDT.Month) {
					lastMonthlyPresence = monthlyPresence;
					monthlyPresence = 0;
				}

				lastPresenceDate = nowDT.ToString();
				monthlyPresence += 1;

				return Save();
			}

			return false;
		}

		public DateTime GetDateTime() {
			return Convert.ToDateTime(lastPresenceDate);
		}

		public bool PresenceLoad() {
			List<GBPresenceObject> collection = GBCollectionService.Instance.GetObjectByName<GBPresenceObject>("presence", "username", username);

			// If Object Already Exist. Replace uuid
			if (collection != null && collection.Count > 0) {
				_count = collection.Count;
				GBPresenceObject item = (GBPresenceObject)collection[0];
				this.uuid 					= item.uuid;
				this.lastPresenceDate 		= item.lastPresenceDate;
				this.monthlyPresence 		= item.monthlyPresence;
				this.lastMonthlyPresence 	= item.lastMonthlyPresence;

				return true;
			}

			return false;
		}
	}
}
