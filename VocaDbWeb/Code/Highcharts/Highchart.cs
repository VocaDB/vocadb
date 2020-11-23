using System.Collections.Generic;
using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts
{
	public class Highchart
	{
		public Chart Chart { get; set; }

		public dynamic Legend { get; set; }

		public PlotOptions PlotOptions { get; set; }

		public IEnumerable<Series> Series { get; set; }

		public Title Title { get; set; }

		[JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
		public dynamic Tooltip { get; set; }

		public Axis XAxis { get; set; }

		public Axis YAxis { get; set; }
	}
}