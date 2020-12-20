#nullable disable

using System.Collections.Generic;
using System.Globalization;

namespace VocaDb.Model.Helpers
{
	/// <summary>
	/// Equality comparer for strings that ignores kana type (Hiragana and Katakana are equivalent) and case.
	/// For example, "コノザマ" and "このざま" are equivalent.
	/// This matches VocaDB's default database collation.
	/// </summary>
	public class KanaAndCaseInsensitiveStringComparer : IEqualityComparer<string>
	{
		private static readonly CompareInfo s_compareInfo = CompareInfo.GetCompareInfo("en-US");
		private const CompareOptions Options = CompareOptions.IgnoreKanaType | CompareOptions.IgnoreCase;

		public bool Equals(string x, string y)
		{
			return s_compareInfo.Compare(x, y, Options) == 0;
		}

		public int GetHashCode(string obj)
		{
			return s_compareInfo.GetHashCode(obj, Options);
		}
	}
}
