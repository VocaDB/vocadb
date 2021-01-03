#nullable disable

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VocaDb.Model.Domain.Globalization
{
	public class RegionCollection
	{
		// FIXME
		public static readonly string[] RegionCodes = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
			.Select(culture => new RegionInfo(culture.Name).TwoLetterISORegionName)
			.OrderBy(c => c)
			.Distinct()
			.ToArray();

		public RegionInfo[] Regions { get; }

		public RegionCollection(string[] regions)
		{
			Regions = regions.Select(r => new RegionInfo(r)).ToArray();
		}

		public Dictionary<string, string> ToDictionaryFull(string defaultName = null)
		{
			return Enumerable
				.Repeat(new KeyValuePair<string, string>(string.Empty, defaultName), defaultName != null ? 1 : 0)
				.Concat(Regions.Select(r => new KeyValuePair<string, string>(r.TwoLetterISORegionName, r.EnglishName))  // TODO: localize
					.OrderBy(k => k.Value))
				.ToDictionary(k => k.Key, k => k.Value);
		}
	}
}
