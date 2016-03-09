using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace VocaDb.Model.Domain.Globalization {

	public static class InterfaceLanguage {

		public static IEnumerable<CultureInfo> Cultures {
			get {
				return LanguageCodes.Select(CultureInfo.GetCultureInfo);
			}
		}

		public static readonly string[] LanguageCodes = {
			"en-US", "de-DE", "fi-Fi", "ru-RU", "ja-JP", "zh-Hans"
		};

	}
}
