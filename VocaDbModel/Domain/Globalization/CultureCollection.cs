#nullable disable

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VocaDb.Model.Domain.Globalization
{
	public class CultureCollection
	{
		public CultureCollection(CultureInfo[] cultures)
		{
			Cultures = cultures;
		}

		public CultureCollection(string[] cultures)
		{
			Cultures = cultures.Select(CultureInfo.GetCultureInfo).ToArray();
		}

		public CultureInfo[] Cultures { get; }

		public Dictionary<string, string> ToDictionaryFull(string defaultName = null)
		{
			return Enumerable
				.Repeat(new KeyValuePair<string, string>(string.Empty, defaultName), defaultName != null ? 1 : 0)
				.Concat(Cultures.Select(c => new KeyValuePair<string, string>(c.Name, c.NativeName + " (" + c.EnglishName + ")"))
					.OrderBy(k => k.Value))
				.ToDictionary(k => k.Key, k => k.Value);
		}
	}
}
