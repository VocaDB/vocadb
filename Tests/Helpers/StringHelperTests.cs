#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{
	[TestClass]
	public class StringHelperTests
	{
		private void TestRemoveControlChars(string expected, string input)
		{
			var actual = StringHelper.RemoveControlChars(input);
			actual.Should().Be(expected);
		}

		private void TestTrimIfNotWhitespace(string expected, string input)
		{
			var actual = StringHelper.TrimIfNotWhitespace(input);
			actual.Should().Be(expected);
		}

		[TestMethod]
		public void RemoveControlCharacter()
		{
			TestRemoveControlChars("Invisible", "\x10Invisible");
		}

		[TestMethod]
		public void RemoveControlCharsKeepSpaces()
		{
			TestRemoveControlChars("Hatsune Miku", "Hatsune Miku");
		}

		[TestMethod]
		public void RemoveControlCharsKeepNewlines()
		{
			TestRemoveControlChars("Hatsune Miku\nVocaloid CV01", "Hatsune Miku\nVocaloid CV01");
		}

		[TestMethod]
		public void RemoveControlCharsKeepNonAscii()
		{
			TestRemoveControlChars("初音ミク", "初音ミク");
		}

		[TestMethod]
		public void TrimIfNotWhitespace_Trim()
		{
			TestTrimIfNotWhitespace("Miku", "    Miku");
		}

		[TestMethod]
		public void TrimIfNotWhitespace_Whitespace()
		{
			TestTrimIfNotWhitespace(" ", " ");
		}
	}
}
