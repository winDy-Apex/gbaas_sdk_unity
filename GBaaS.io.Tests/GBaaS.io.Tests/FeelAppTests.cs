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
		// 스코어 저장하기, stage = 영웅대전 구분용, unit = 캐릭터 별 구분용
		public void SaveScore() {
			// GBaaS 를 사용하기 위한 로그인
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// GBaaS 에 스코어를 등록
			var result = _gbaas.AddScore (new GBScoreObject {
				stage = "0",
				score = 500,
				unit = "전사" 
			});
			Assert.IsTrue(result);

			// GBaaS 에 스코어를 등록
			result = _gbaas.AddScore (new GBScoreObject {
				stage = "영웅대전1",
				score = 200,
				unit = "내복맨" 
			});
			Assert.IsTrue(result);

			// GBaaS 에 스코어를 등록
			result = _gbaas.AddScore (new GBScoreObject {
				stage = "영웅대전2",
				score = 300,
				unit = "울트라맨" 
			});
			Assert.IsTrue(result);
		}

		[Test]
		// 전체랭킹
		public void GetScoreAll() {
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, 최대 Limit 지정한만큼 가져옴, 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result = _gbaas.GetScore ("", "", 5);
			Console.Out.WriteLine ("GetScoreAll Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);

			// 화면에 한세트를 출력 후 다음 Limit 지정한만큼 가져옴(for 페이지네이션), 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result2 = _gbaas.GetScoreMore ("", "", 5, true);
			Console.Out.WriteLine ("GetScoreAll Result More Count : " + result2.Count.ToString ());
			Assert.IsNotNull (result2);
		}

		[Test]
		// 영웅대전 랭킹
		public void GetScoreByHero() {
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, stage 에 영웅대전 이름을 입력하여 해당 영웅대전의 점수만 가져온다.
			List<Objects.GBScoreObject> result = _gbaas.GetScore ("영웅대전1", "", 5);
			Console.Out.WriteLine ("GetScore of 영웅대전 Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		// 캐릭터별 랭킹
		public void GetScoreByCharacter() {
			var login = _gbaas.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, unit 에 캐릭터 이름을 입력하여 해당 캐릭터의 점수만 가져온다.
			List<Objects.GBScoreObject> result = _gbaas.GetScore ("", "울트라맨", 5);
			Console.Out.WriteLine ("GetScore of 캐릭터 Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		// 자기등수 구하기
		public void GetRank() {
			var login = _gbaas.Login(Defines.TEST_USERNAME1, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, 최대 Limit 지정한만큼 가져옴, 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result0 = _gbaas.GetRank ("", "", ScoreOrder.DESC, Period.Monthly, 1, 10);
			Console.Out.WriteLine ("GetRank Result Count : " + result0.Count.ToString ());
			Assert.IsNotNull (result0);

			// 자기 등수 가져오기
			List<Objects.GBScoreObject> result = _gbaas.GetRank("", "", ScoreOrder.DESC, Period.Monthly);
			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetRank Result My Rank : " + result[0].rank.ToString());

			// 지정 등수 주변 가져오기
			List<Objects.GBScoreObject> result1 = _gbaas.GetRank("", "", ScoreOrder.DESC, Period.Monthly, result[0].rank, 3);
			Assert.IsNotNull (result1);
			Console.Out.WriteLine ("GetRank Result Count : " + result1.Count.ToString ());

			// 자기 등수 주변 바로 가져오기
			List<Objects.GBScoreObject> result2 = _gbaas.GetRank("", "", ScoreOrder.DESC, Period.Monthly, 0, 3);
			Assert.IsNotNull (result2);
			Console.Out.WriteLine ("GetRank Result Count : " + result2.Count.ToString ());
		}

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
