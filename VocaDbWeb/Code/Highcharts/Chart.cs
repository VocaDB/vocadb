using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Web.Code.Highcharts {

	public class Chart {

		public int? Height { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		public ChartType Type { get; set; }

	}

}