using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using NUnit.Framework;

// 필앱 요청 사항 스코어 활용 가이드 겸 테스트 코드
// 테스트는 동기식으로 동작합니다. 실제 코드에서는 GBaaS의 비동기 연동을 참고하시어 구현하시면 됩니다.
namespace GBaaS.io.Tests {
	public class FeelAppTests {
		GBaaS.io.GBaaSApi _gbaas = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL3);

		[Test]
		// 출석부 사용하기
		public void PresenceTest() {
			var login = _gbaas.Login(Defines.TEST_USERNAME1, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			GBPresenceObject presence = new GBPresenceObject();

			// 인위적으로 마지막 출석 날짜를 조작하면 출석횟수가 증가한다.
			bool result = presence.DoPresence("2014-09-01 오후 3:49:35");
			Assert.IsTrue(result);

			// 같은 날이면 출석 횟수 변동이 없어야 한다.
			result = presence.DoPresence();
			Assert.IsFalse(result);

			// 달이 바뀌면 출석횟수가 리셋되어야한다.
			result = presence.DoPresence("2014-01-01 오후 3:49:35");
			Assert.IsTrue(presence.monthlyPresence == 1);
		}
	}
}
