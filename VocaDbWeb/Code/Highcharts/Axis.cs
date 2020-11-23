using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts
{

	public class Axis
	{

		public Axis() { }

		public Axis(AxisType type, Title title)
		{
			Type = type;
			Title = title;
		}

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public object Min { get; set; }

		public Title Title { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public AxisType? Type { get; set; }

	}

	public enum AxisType
	{

		Datetime

	}

}