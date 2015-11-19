namespace VocaDb.Model.DataContracts.Aggregate {

	public class CountPerDayContract {

		public CountPerDayContract() {
			Day = 1;
		}

		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public int Count { get; set; }
	}

}