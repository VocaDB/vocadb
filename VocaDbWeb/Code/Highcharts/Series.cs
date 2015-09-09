using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts {

	public class Series {

		public object[][] Data { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public int? LineWidth { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public dynamic Marker { get; set; }

		public string Name { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public SeriesType? Type { get; set; }

	}

}