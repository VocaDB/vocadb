using System;
using System.Collections.Generic;

namespace VocaDb.Model.Helpers {

	/// <summary>
	/// Code from: https://www.zhihu.com/question/381784377/answer/1099438784
	/// </summary>
	public static class BilibiliHelper {

		private const string table = "fZodR9XQDSUm21yCkr6zBqiveYah8bt4xsWpHnJE7jL5VG3guMTKNPAwcF";
		private const int xor = 177451812;
		private const long add = 8728348608L;

		private static readonly Dictionary<char, int> tr = new Dictionary<char, int>();
		private static readonly int[] s = new[] { 11, 10, 3, 8, 4, 6 };

		static BilibiliHelper() {
			for (var i = 0; i < table.Length; i++)
				tr[table[i]] = i;
		}

		public static long Decode(string x) {
			var r = 0L;

			for (var i = 0; i < s.Length; i++)
				r += tr[x[s[i]]] * (long)Math.Pow(table.Length, i);

			return (r - add) ^ xor;
		}

		public static string Encode(long x) {
			x = (x ^ xor) + add;
			var r = new[] { 'B', 'V', '1', ' ', ' ', '4', ' ', '1', ' ', '7', ' ', ' ' };

			for (var i = 0; i < s.Length; i++)
				r[s[i]] = table[(int)(x / Math.Pow(table.Length, i) % table.Length)];

			return string.Join("", r);
		}

	}

}
