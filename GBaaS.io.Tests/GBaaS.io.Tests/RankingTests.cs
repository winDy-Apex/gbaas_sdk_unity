using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GBaaS.io.Objects;
using GBaaS.io.Services;
using NUnit.Framework;

namespace GBaaS.io.Tests
{
	public class RankingTests {
		GBaaS.io.GBaaSApi gbaasApi = new GBaaS.io.GBaaSApi(Defines.USERGRID_URL3);

		[Test]
		public void AddScoreTest() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// Name must be unique.
			var result = gbaasApi.AddScore (new GBScoreObject {
				stage = "1st",
				score = 1201,
				unit = "point" 
			});

			Assert.IsTrue(result);
		}

		[Test]
		public void GetScoreAndUpdateScoreAndSetLocation() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("1st", "point", 0, "");

			Console.Out.WriteLine ("GetScoreTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);

			Console.Out.WriteLine ("GetScoreTest Result UUID : " + result[0].GetUUID());
			List<Objects.GBScoreObject> result2 = gbaasApi.GetScoreByUuidOrName (result[0].GetUUID());

			Console.Out.WriteLine ("GetScoreTest Result2 UUID : " + result2[0].GetUUID());
			Assert.IsTrue(result[0].GetUUID().CompareTo(result2[0].GetUUID()) == 0);

			// Name must be unique.
			GBScoreObject scoreObject = new GBScoreObject {
				stage = "1st",
				score = 190,
				unit = "point"
			};
			scoreObject.SetUUID(result[0].GetUUID());

			var result3 = gbaasApi.AddScore (scoreObject);

			Assert.IsTrue(result3);

			var result4 = scoreObject.SetLocation(1.33f, 2.1f);

			Assert.IsTrue(result4);
		}

		[Test]
		public void GetScoreTest() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScore("1st", "point", 0, "");

			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetScoreTest Result Count : " + result.Count.ToString ());
		}

		[Test]
		public void GetScoreLogTest() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScoreLog("", "", 10, "");

			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());
		}

		[Test]
		public void GetScoreLogMoreTest() {
			var login = gbaasApi.Login("test", Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScoreLogMore("", "", 10, false);

			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());

			result = gbaasApi.GetScoreLogMore("", "", 10, true);

			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetScoreLogTest Result Count : " + result.Count.ToString ());

		}

		[Test]
		public void GetScoreLogWithoutStageUnitInfoTest() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScoreLog("", "", 0, "");

			Console.Out.WriteLine ("GetScoreLogWithoutStageUnitInfoTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreWeekly() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);
			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("", "", 10, "", GBaaS.io.Period.Weekly, DayOfWeek.Sunday);

			Console.Out.WriteLine ("GetScoreWeekly Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreDaily() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("", "", 10, "", GBaaS.io.Period.Daily);

			Console.Out.WriteLine ("GetScoreDaily Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetScoreUser() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetScoreLog("", "", 10, "", GBaaS.io.Period.Daily);

			Console.Out.WriteLine ("GetScoreUser Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void GetRank() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			List<Objects.GBScoreObject> result = gbaasApi.GetRank("", "", ScoreOrder.DESC, Period.Monthly, 0, 5);

			Console.Out.WriteLine ("GetRank Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		public void AddScoreTestWithStage() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// Name must be unique.
			var result = gbaasApi.AddScore (new GBScoreObject {
				stage = "ABC",
				score = 100,
				unit = "point" 
			});

			Assert.IsTrue(result);
		}

		[Test]
		public void GetScoreTestWithStage() {
			List<Objects.GBScoreObject> result = gbaasApi.GetScore("ABC", "", 10, "");

			Console.Out.WriteLine ("GetScoreTest Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		// 스코어 저장하기, stage = 영웅대전 구분용, unit = 캐릭터 별 구분용
		public void SaveScore() {
			// GBaaS 를 사용하기 위한 로그인
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// GBaaS 에 스코어를 등록
			var result = gbaasApi.AddScore (new GBScoreObject {
				stage = "0",
				score = 500,
				unit = "전사" 
			});
			Assert.IsTrue(result);

			// GBaaS 에 스코어를 등록
			result = gbaasApi.AddScore (new GBScoreObject {
				stage = "영웅대전1",
				score = 200,
				unit = "내복맨" 
			});
			Assert.IsTrue(result);

			// GBaaS 에 스코어를 등록
			result = gbaasApi.AddScore (new GBScoreObject {
				stage = "영웅대전2",
				score = 300,
				unit = "울트라맨" 
			});
			Assert.IsTrue(result);
		}

		[Test]
		// 전체랭킹
		public void GetScoreAll() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, 최대 Limit 지정한만큼 가져옴, 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("", "", 5);
			Console.Out.WriteLine ("GetScoreAll Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);

			// 화면에 한세트를 출력 후 다음 Limit 지정한만큼 가져옴(for 페이지네이션), 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result2 = gbaasApi.GetScoreMore ("", "", 5, true);
			Console.Out.WriteLine ("GetScoreAll Result More Count : " + result2.Count.ToString ());
			Assert.IsNotNull (result2);
		}

		[Test]
		// 영웅대전 랭킹
		public void GetScoreByHero() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, stage 에 영웅대전 이름을 입력하여 해당 영웅대전의 점수만 가져온다.
			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("영웅대전1", "", 5);
			Console.Out.WriteLine ("GetScore of 영웅대전 Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		// 캐릭터별 랭킹
		public void GetScoreByCharacter() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, unit 에 캐릭터 이름을 입력하여 해당 캐릭터의 점수만 가져온다.
			List<Objects.GBScoreObject> result = gbaasApi.GetScore ("", "울트라맨", 5);
			Console.Out.WriteLine ("GetScore of 캐릭터 Result Count : " + result.Count.ToString ());
			Assert.IsNotNull (result);
		}

		[Test]
		// 자기등수 구하기
		public void GetUserRank() {
			var login = gbaasApi.Login(Defines.TEST_USERNAME, Defines.TEST_PASSWORD);
			Assert.IsNotNull(login);

			// 점수 가져오기, 최대 Limit 지정한만큼 가져옴, 스코어 순으로 기본적으로 소팅됨
			List<Objects.GBScoreObject> result0 = gbaasApi.GetRank ("", "", ScoreOrder.DESC, Period.Monthly, 1, 10);
			Console.Out.WriteLine ("GetRank Result Count : " + result0.Count.ToString ());
			Assert.IsNotNull (result0);

			// 자기 등수 가져오기
			List<Objects.GBScoreObject> result = gbaasApi.GetRank("", "", ScoreOrder.DESC, Period.Monthly);
			Assert.IsNotNull (result);
			Console.Out.WriteLine ("GetRank Result My Rank : " + result[0].rank.ToString());

			// 지정 등수 주변 가져오기
			List<Objects.GBScoreObject> result1 = gbaasApi.GetRank("", "", ScoreOrder.DESC, Period.Monthly, result[0].rank, 3);
			Assert.IsNotNull (result1);
			Console.Out.WriteLine ("GetRank Result Count : " + result1.Count.ToString ());

			// 자기 등수 주변 바로 가져오기
			List<Objects.GBScoreObject> result2 = gbaasApi.GetRank("", "", ScoreOrder.DESC, Period.Monthly, 0, 3);
			Assert.IsNotNull (result2);
			Console.Out.WriteLine ("GetRank Result Count : " + result2.Count.ToString ());
		}

		[Test]
		public void GetTimeStamp() {
			Console.Out.WriteLine ("miliseconds: " + gbaasApi.GetTimeStamp ());
			Console.Out.WriteLine ("daily: " + gbaasApi.GetTimeStamp (Period.Daily));
			Console.Out.WriteLine ("weekly: " + gbaasApi.GetTimeStamp (Period.Weekly, DayOfWeek.Sunday));

			Assert.LessOrEqual(ulong.Parse(gbaasApi.GetTimeStamp (Period.Daily)), ulong.Parse(gbaasApi.GetTimeStamp ()));
			Assert.LessOrEqual(ulong.Parse(gbaasApi.GetTimeStamp (Period.Weekly, DayOfWeek.Sunday)), ulong.Parse(gbaasApi.GetTimeStamp (Period.Daily)));
		}
	}
}
