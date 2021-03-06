using System;
using GBaaS.io.Utils;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace GBaaS.io.Services
{
	class GBScoreService : GBService<GBScoreService>
	{
		public GBScoreService() {}

		public List<GBaaSApiHandler> _handler = new List<GBaaSApiHandler>();

		// Cursor For Continuos Query
		private string _cursor_score 	= "";
		private string _cursor_scoreLog	= "";

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

		public GBResult AddScore(Objects.GBScoreObject score) {
			if (IsAsync()) {
				(new Thread(() => this.AddScoreThread(score))).Start();
				return new GBResult { isSuccess = false, returnCode = ReturnCode.WaitAsync, reason = "Wait Async Request" };
			} else {
				return this.AddScoreThread(score);
			}
		}

		private GBResult AddScoreThread(Objects.GBScoreObject score) {
			GBResult result = new GBResult();

			try {
				result = score.Update();
			} catch (Exception ex) {
				result.MakeResult(false, ReturnCode.Exception, ex.ToString());
			}

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnAddScore(result);
				}
			}

			return result;
		}

		public List<Objects.GBScoreObject> GetScoreByUuidOrName(string uuidOrName = "") {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetScoreByUuidOrNameThread(uuidOrName));
				workerThread.Start();
				return default(List<Objects.GBScoreObject>);
			} else {
				return this.GetScoreByUuidOrNameThread(uuidOrName);
			}
		}

		private List<Objects.GBScoreObject> GetScoreByUuidOrNameThread(string uuidOrName = "") {
			string entity_type = "leaderboard";

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + entity_type + "/" + uuidOrName, HttpHelper.RequestTypes.Get, "");
			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetScoreByUuidOrName(default(List<Objects.GBScoreObject>));
					}
				} else {
					return default(List<Objects.GBScoreObject>);
				}
			}

			var scores = GBRequestService.Instance.GetEntitiesFromJson(rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetScoreByUuidOrName(MakeScoreList(scores));
				}
			} else {
				return MakeScoreList(scores);
			}

			return default(List<Objects.GBScoreObject>);
		}

		/// <summary>
		/// Gets the score.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		/// 
		public List<Objects.GBScoreObject> GetScoreMore(string stage = "", string unit = "", int limit = 0, bool isMore = false, Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			if (isMore) {
				return GetScore(stage, unit, limit, _cursor_score, period, weekstart);
			}

			return GetScore(stage, unit, limit, "", period, weekstart);
		}


		public List<Objects.GBScoreObject> GetScore(string stage = "", string unit = "", 
			int limit = 0, string cursor = "", 
			Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetScoreThread(stage, unit, limit, cursor, period, weekstart));
				workerThread.Start();
				return MakeErrorList(false, ReturnCode.WaitAsync, "Wait Async Request");
			} else {
				return this.GetScoreThread(stage, unit, limit, cursor, period, weekstart);
			}
		}

		/// <summary>
		/// Gets the score.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="cursor">Limit 검색범위를 넘은 데이터를 조회할 수 있는 Cursor값</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		private List<Objects.GBScoreObject> GetScoreThread(string stage = "", string unit = "", 
													int limit = 0, string cursor = "", 
													Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			string entity_type = "leaderboard";

			string query = "";

			if (stage.Length > 0) {
				query += "stage=" + stage;
			}

			if (unit.Length > 0) {
				if (stage.Length > 0) {
					query += "&";
				}
				query += "unit=" + unit;
			}

			if (limit > 0) {
				if ((stage.Length + unit.Length) > 0) {
					query += "&";
				}
				query += "limit=" + limit.ToString();
			}

			if (cursor.Length > 0) {
				if ((stage.Length + unit.Length + limit) > 0) {
					query += "&";
				}
				query += "cursor=" + cursor;
			}

			string rawResults = "";
			try {
				rawResults = GBRequestService.Instance.PerformRequest<string>("/" + entity_type + "?" + query, HttpHelper.RequestTypes.Get, "");
			} catch (Exception ex) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetScore(MakeErrorList(false, ReturnCode.Exception, ex.ToString()));
					}
				}

				return MakeErrorList(false, ReturnCode.Exception, ex.ToString());
			}

			if (rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetScore(default(List<Objects.GBScoreObject>));
					}
				} else {
					return default(List<Objects.GBScoreObject>);
				}
			}

			var scores 		= GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			_cursor_score	= GBRequestService.Instance.GetValueFromJson("cursor", rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetScore(MakeScoreList(scores));
				}
			} else {
				return MakeScoreList(scores);
			}

			return default(List<Objects.GBScoreObject>);
		}
			
		public List<Objects.GBScoreObject> GetRank(string stage = "", string unit = "", 
			ScoreOrder scoreOrder = ScoreOrder.DESC, Period period = Period.Daily, int rank = 0, int range = 1) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetRankThread(stage, unit, scoreOrder, period, rank, range));
				workerThread.Start();
				return default(List<Objects.GBScoreObject>);
			} else {
				return this.GetRankThread(stage, unit, scoreOrder, period, rank, range);
			}
		}

		/// <summary>
		/// Gets the rank thread.
		/// </summary>
		/// <returns>The rank thread.</returns>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="scoreOrder">Score order.</param>
		/// <param name="period">Period.</param>
		/// <param name="rank">Rank.</param>
		/// <param name="range">Range.</param>
		private List<Objects.GBScoreObject> GetRankThread(string stage = "", string unit = "", 
			ScoreOrder scoreOrder = ScoreOrder.DESC, Period period = Period.Daily, int rank = 0, int range = 1) {
			string entity_type = "leaderboard/ranks";

			string query = "";

			if (stage.Length > 0) {
				query += "stage=" + stage;
			}

			if (unit.Length > 0) {
				if (stage.Length > 0) {
					query += "&";
				}
				query += "unit=" + unit;
			}

			if (scoreOrder.ToString().Length > 0) {
				if ((stage.Length + unit.Length) > 0) {
					query += "&";
				}
				query += "scoreOrder=" + scoreOrder.ToString();
			}

			if (period.ToString().Length > 0) {
				if ((stage.Length + unit.Length + scoreOrder.ToString().Length) > 0) {
					query += "&";
				}
				query += "period=" + period.ToString();
			}

			if ((stage.Length + unit.Length + scoreOrder.ToString().Length + period.ToString().Length) > 0) {
				query += "&";
			}

			query += "rank=" + rank.ToString() + "&range=" + range.ToString();

			string requestUrl = "/" + entity_type + "?" + query;

			// If PerformRequest failure, It returns null
			var rawResults = GBRequestService.Instance.PerformRequest<string>(requestUrl, HttpHelper.RequestTypes.Get, "");
			if (rawResults != null && rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetRank(default(List<Objects.GBScoreObject>));
					}
				} else {
					return default(List<Objects.GBScoreObject>);
				}
			}

			var scores 		= GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			_cursor_score	= GBRequestService.Instance.GetValueFromJson("cursor", rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetRank(MakeScoreList(scores));
				}
			} else {
				return MakeScoreList(scores);
			}

			return default(List<Objects.GBScoreObject>);
		}

		public List<Objects.GBScoreObject> GetScoreLogMore(string stage = "", string unit = "", 
			int limit = 0, bool isMore = false, 
			Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			if (isMore) {
				return this.GetScoreLog(stage, unit, limit, _cursor_scoreLog, period, weekstart);
			}

			return this.GetScoreLog(stage, unit, limit, "", period, weekstart);
		}

		/// <summary>
		/// Gets the score.
		/// </summary>
		/// <returns>The score.</returns>
		/// <param name="name">Name.</param>
		/// <param name="stage">Stage.</param>
		/// <param name="unit">Unit.</param>
		/// <param name="limit">Limit.</param>
		/// <param name="cursor">Limit 검색범위를 넘은 데이터를 조회할 수 있는 Cursor값</param>
		/// <param name="period">점수를 구하는 기간을 지정한다. 주간, 일간 지원</param>
		/// <param name="weekstart">주간 단위로 점수를 구할 경우 시작하는 주의 첫 요일 지정</param>
		public List<Objects.GBScoreObject> GetScoreLog(string stage = "", string unit = "", 
			int limit = 0, string cursor = "", 
			Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			if (IsAsync()) {
				Thread workerThread = new Thread(() => this.GetScoreLogThread(stage, unit, limit, cursor, period, weekstart));
				workerThread.Start();
				return default(List<Objects.GBScoreObject>);
			} else {
				return this.GetScoreLogThread(stage, unit, limit, cursor, period, weekstart);
			}
		}

		private List<Objects.GBScoreObject> GetScoreLogThread(string stage = "", string unit = "", 
			int limit = 0, string cursor = "", 
			Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			string entity_type = "leaderboard/me/logs";

			string query = "";

			if (stage.Length > 0) {
				query += "stage=" + stage;
			}

			if (unit.Length > 0) {
				if (stage.Length > 0) {
					query += "&";
				}
				query += "unit=" + unit;
			}

			if (limit > 0) {
				if ((stage.Length + unit.Length) > 0) {
					query += "&";
				}
				query += "limit=" + limit.ToString();
			}

			if (cursor.Length > 0) {
				if ((stage.Length + unit.Length + limit) > 0) {
					query += "&";
				}
				query += "cursor=" + cursor;
			}

			var rawResults = GBRequestService.Instance.PerformRequest<string>("/" + entity_type + "?" + query, HttpHelper.RequestTypes.Get, "");
			if (rawResults == null || rawResults.IndexOf ("error") != -1) {
				if (IsAsync()) {
					foreach (GBaaSApiHandler handler in _handler) {
						handler.OnGetScoreLog(default(List<Objects.GBScoreObject>));
					}
				}

				return default(List<Objects.GBScoreObject>);
			}

			var scores			= GBRequestService.Instance.GetEntitiesFromJson(rawResults);
			_cursor_scoreLog 	= GBRequestService.Instance.GetValueFromJson("cursor", rawResults);

			if (IsAsync()) {
				foreach (GBaaSApiHandler handler in _handler) {
					handler.OnGetScoreLog(MakeScoreList(scores));
				}
			} else {
				return MakeScoreList(scores);
			}

			return default(List<Objects.GBScoreObject>);
		}

		public string GetTimeStamp(Period period = Period.Alltime, DayOfWeek weekstart = DayOfWeek.Sunday) {
			//Find unix timestamp (seconds since 01/01/1970)
			long ticks = DateTime.UtcNow.Ticks - DateTime.Parse ("01/01/1970 00:00:00").Ticks;
			ticks /= 10000; //Convert windows ticks to milliseconds

			long seconds = ticks / 1000;
			long minute = seconds / 60;
			long hours = minute / 60;
			long days = hours / 24;

			long timestamp = ticks;

			if (period == Period.Daily) {

				timestamp = days * 24 * 60 * 60 * 1000;

			} else if (period == Period.Weekly) {

				int daysDiff = weekstart - DateTime.Today.DayOfWeek;
				if (daysDiff > 0) {
					// 한주를 넘어가면 7을 빼서 주간 일차를 보정한다.
					daysDiff -= 7;
				}
				//Console.Out.WriteLine ("daysDiff: " + daysDiff.ToString ());
				
				timestamp = (days + daysDiff) * 24 * 60 * 60 * 1000;
			}

			return timestamp.ToString ();
		}

		private List<Objects.GBScoreObject> MakeScoreList(Newtonsoft.Json.Linq.JToken scores) {
			List<Objects.GBScoreObject> results = new List<Objects.GBScoreObject>();
			foreach (var score in scores)
			{
				Objects.GBScoreObject scoreObject = JsonConvert.DeserializeObject<Objects.GBScoreObject>(score.ToString());

				if (scoreObject.displayName == null || scoreObject.displayName.Length <= 0) {
					scoreObject.displayName = scoreObject.username;
				}

				scoreObject.MakeResult(true, ReturnCode.Success, "");
				results.Add(scoreObject);
			}

			return results;
		}
			
		private List<Objects.GBScoreObject> MakeErrorList(bool isSuccess, ReturnCode returnCode, string reason) {
			List<Objects.GBScoreObject> results = new List<Objects.GBScoreObject>();

			Objects.GBScoreObject scoreObject = new Objects.GBScoreObject();
			scoreObject.MakeResult(isSuccess, returnCode, reason);

			results.Add(scoreObject);

			return results;
		}
	}
}
