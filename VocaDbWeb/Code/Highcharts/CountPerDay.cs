namespace VocaDb.Web.Code.Highcharts {

	public class CountPerDay {

		public CountPerDay() {
			Day = 1;
		}

		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public int Count { get; set; }
	}

}