
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace VocaDb.Model.Domain.Globalization
{

	public readonly struct LanguageCodeAndName
	{

		public static LanguageCodeAndName[] Languages(IEnumerable<CultureInfo> cultures, string defaultName = null)
		{

			return Enumerable
				.Repeat(new LanguageCodeAndName(string.Empty, defaultName), defaultName != null ? 1 : 0)
				.Concat(cultures.Select(c => new LanguageCodeAndName(c.Name, c.NativeName + " (" + c.EnglishName + ")"))
					.OrderBy(k => k.DisplayName))
				.ToArray();

		}

		public LanguageCodeAndName(string code, string displayName)
		{
			Code = code;
			DisplayName = displayName;
		}

		public string Code { get; }

		public string DisplayName { get; }

	}

}
