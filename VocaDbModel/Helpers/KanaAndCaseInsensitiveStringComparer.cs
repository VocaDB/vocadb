using System.Collections.Generic;
using System.Globalization;

namespace VocaDb.Model.Helpers {

	/// <summary>
	/// Equality comparer for strings that ignores kana type (Hiragana and Katakana are equivalent) and case.
	/// For example, "コノザマ" and "このざま" are equivalent.
	/// This matches VocaDB's default database collation.
	/// </summary>
	public class KanaAndCaseInsensitiveStringComparer : IEqualityComparer<string> {

		private static readonly CompareInfo compareInfo = CompareInfo.GetCompareInfo("en-US");
		private const CompareOptions options = CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase;

		public bool Equals(string x, string y) {
			return compareInfo.Compare(x, y, options) == 0;
		}

		public int GetHashCode(string obj) {
			return compareInfo.GetHashCode(obj, options);
		}

	}

}
