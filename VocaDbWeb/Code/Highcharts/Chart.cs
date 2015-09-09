using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts {

	public class Chart {

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public string BackgroundColor { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? Height { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		public ChartType Type { get; set; }

	}

}