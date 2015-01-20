/**
 * GBUniqueObject GBaaS Data Store 에 User 당 유니크하게 존재 하는 것을 <br>
 * 보장하는 오브젝트 주로 게임 상태 저장 등의 용도로 사용한다.
 */
using System;
using System.Collections.Generic;
using GBaaS.io.Services;

namespace GBaaS.io.Objects {
	// GBObject 
	// 일반 오브젝트와 같지만 Save 할때 같은 오브젝트가 있으면 업데이트 한다.
	public class GBUniqueObject : GBObject {

		public int 		_count; 				// Just For TDD, not recommend to use.
		public string 	username { get; set; }	// UniqueObject Unique per User. Same Name On GBaaS Collection Entity Field so, not "_userName" Style, just username.

		public GBUniqueObject() {
			username = GBUserService.Instance.GetLoginName();
		}

		public override GBResult Save() {
			if (Load() == false) {
				return base.Save();
			}

			return Update();
		}

		public override bool Load() {
			List<GBObject> collection = GBCollectionService.Instance.GetListByKey(this.GetEntityType(), "username", username);

			// If Object Already Exist. Replace uuid
			if (collection != null && collection.Count > 0) {
				_count = collection.Count;
				this.uuid = collection[0].uuid;
				return true;
			}

			return false;
		}
	}
}
