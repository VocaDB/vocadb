using Newtonsoft.Json;

namespace VocaDb.Web.Code.Highcharts {

	public class PlotOptions {

		public PlotOptionsArea Area { get; set; }

		public dynamic Bar { get; set; }

	}

	public class PlotOptionsArea {
		
		public string LineColor { get; set; }

		public double LineWidth { get; set; }

		public dynamic Marker { get; set; }

		[JsonConverter(typeof(CamelCaseStringEnumConverter))]
		public PlotOptionsAreaStacking? Stacking { get; set; }

	}

	public enum PlotOptionsAreaStacking {
		
		Percent

	}

}