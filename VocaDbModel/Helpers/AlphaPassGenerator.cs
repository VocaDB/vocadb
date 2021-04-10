using System;
using System.Linq;

namespace VocaDb.Model.Helpers
{
	public class AlphaPassGenerator
	{
		private readonly char[] _allowedChars;
		private readonly Random _random;

		private char GenerateRandomChar()
		{
			return _allowedChars[_random.Next(_allowedChars.Length)];
		}

		public AlphaPassGenerator(bool ucChar, bool lcChar, bool digits)
		{
			_random = new Random();

			_allowedChars =
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
