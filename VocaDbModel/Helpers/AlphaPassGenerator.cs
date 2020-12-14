#nullable disable

using System;
using System.Linq;

namespace VocaDb.Model.Helpers
{
	public class AlphaPassGenerator
	{
		private readonly char[] allowedChars;
		private readonly Random random;

		private char GenerateRandomChar()
		{
			return allowedChars[random.Next(allowedChars.Length)];
		}

		public AlphaPassGenerator(bool ucChar, bool lcChar, bool digits)
		{
			this.random = new Random();

			allowedChars =
				(ucChar ? Enumerable.Range('A', 'Z' - 'A' + 1).Select(c => (char)c) : Enumerable.Empty<char>()).Concat
				(lcChar ? Enumerable.Range('a', 'z' - 'a' + 1).Select(c => (char)c) : Enumerable.Empty<char>()).Concat
				(digits ? Enumerable.Range('0', '9' - '0' + 1).Select(c => (char)c) : Enumerable.Empty<char>())
				.ToArray();
		}

		public string Generate(int length)
		{
			if (length <= 0)
				return string.Empty;

			return new string(
				Enumerable.Range(0, length)
				.Select(i => GenerateRandomChar())
				.ToArray());
		}
	}
}
