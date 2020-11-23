using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts
{

	public class PlotOptions
	{

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public PlotOptionsArea Area { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public dynamic Bar { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public dynamic Pie { get; set; }

	}

	public class PlotOptionsArea
	{

		public string LineColor { get; set; }

		public double LineWidth { get; set; }

		public dynamic Marker { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		public PlotOptionsAreaStacking? Stacking { get; set; }

	}

	public enum PlotOptionsAreaStacking
	{

		Percent

	}

}