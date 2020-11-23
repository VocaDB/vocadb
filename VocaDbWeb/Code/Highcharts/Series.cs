using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using VocaDb.Model.DataContracts.Aggregate;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Code.Highcharts
{

	public class Series
	{

		public static object[][] DateData<T>(IEnumerable<T> source, Func<T, DateTime> dateSelector, Func<T, int> valSelector)
		{
			return source.Select(p => new object[] { HighchartsHelper.ToEpochTime(dateSelector(p)), valSelector(p) }).ToArray();
		}

		public static object[][] DateData(IEnumerable<CountPerDayContract> source)
		{
			return DateData(source, d => d.ToDateTime(), d => d.Count);
		}

		public Series() { }

		public Series(string name, object[][] data, SeriesType? type = null)
		{
			Name = name;
			Data = data;
			Type = type;
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