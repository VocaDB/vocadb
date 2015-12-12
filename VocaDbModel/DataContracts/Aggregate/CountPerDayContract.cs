using System;

namespace VocaDb.Model.DataContracts.Aggregate {

	public class CountPerDayContract {

		public CountPerDayContract() {
			Day = 1;
		}

		public CountPerDayContract(DateTime dt, int count) {
			Year = dt.Year;
			Month = dt.Month;
			Day = dt.Day;
			Count = count;
		}

		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public int Count { get; set; }
	}

}