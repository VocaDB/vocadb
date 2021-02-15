using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Globalization
{
	public class RegionCollection
	{
		public static readonly IEnumerable<RegionInfo> DefaultRegions = CultureInfo.GetCultures(CultureTypes.SpecificCultures)
			.Select(culture => new RegionInfo(culture.Name))
			.Distinct(region => region.TwoLetterISORegionName)
			.OrderBy(region => region.EnglishName)
			.ToArray();

		public IEnumerable<RegionInfo> Regions { get; }

		public RegionCollection(IEnumerable<RegionInfo> regions)
		{
			Regions = regions;
		}

		public IReadOnlyDictionary<string, string?> ToDictionaryFull(string? defaultName = null) => Enumerable
			.Repeat(new KeyValuePair<string, string?>(string.Empty, defaultName), defaultName != null ? 1 : 0)
			.Concat(Regions.Select(r => new KeyValuePair<string, string?>(r.TwoLetterISORegionName, r.EnglishName/* TODO: localize */)).OrderBy(k => k.Value))
			.ToDictionary(k => k.Key, k => k.Value);
	}
}
