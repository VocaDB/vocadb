using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts {

	public class Axis {

		public object Min { get; set; }

		public Title Title { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		public AxisType? Type { get; set; }

	}

	public enum AxisType {
		
		Datetime

	}

}