using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Highcharts {

	public class Series {

		public static object[][] DateData<T>(IEnumerable<T> source, Func<T, DateTime> dateSelector, Func<T, int> valSelector) {
			return source.Select(p => new object[] { HighchartsHelper.ToEpochTime(dateSelector(p)), valSelector(p) }).ToArray();
        }

		public static object[][] DateData(IEnumerable<CountPerDay> source) {
			return DateData(source, d => new DateTime(d.Year, d.Month, d.Day), d => d.Count);
		}

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