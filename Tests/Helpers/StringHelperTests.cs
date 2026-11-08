#nullable disable

using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers;

[TestClass]
public class StringHelperTests
{
	private void TestRemoveControlChars(string expected, string input)
	{
		var actual = StringHelper.RemoveControlChars(input);
		actual.Should().Be(expected);
	}

	private void TestRemoveInvalidXmlChars(string expected, string input)
	{
		var actual = StringHelper.RemoveInvalidXmlChars(input);
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
	public void RemoveInvalidXmlChars_RemovesRawCharacters()
	{
		TestRemoveInvalidXmlChars("Invisible", "\x00\x01\x08\x0B\x0C\x0E\x1FInvisible");
	}

	[TestMethod]
	public void RemoveInvalidXmlChars_KeepsWhitespaceAndUnicode()
	{
		TestRemoveInvalidXmlChars("初音ミク\t\n\r", "初音ミク\t\n\r");
	}

	[TestMethod]
	public void CleanInvalidXmlChars_RemovesInvalidReferences()
	{
		var actual = StringHelper.CleanInvalidXmlChars("a&#x8;b&#x9;c&#x1F;d");

		actual.Should().Be("ab&#x9;cd");
	}

	[TestMethod]
	public void CleanInvalidXmlChars_KeepsValidReferences()
	{
		var actual = StringHelper.CleanInvalidXmlChars("a&#x9;b&#xA;c&#xD;d");

		actual.Should().Be("a&#x9;b&#xA;c&#xD;d");
	}

	[TestMethod]
	public void CleanInvalidXmlChars_RemovesRawInvalidCharacters()
	{
		var actual = StringHelper.CleanInvalidXmlChars("\u0008Miku");

		actual.Should().Be("Miku");
	}

	[TestMethod]
	public void RemoveInvalidXmlChars_EveryUtf16CodeUnit()
	{
		for (var codePoint = 0; codePoint <= 0xFFFF; codePoint++)
		{
			var input = $"a{(char)codePoint}b";
			var valid = codePoint is 0x9 or 0xA or 0xD
				|| codePoint is >= 0x20 and <= 0xD7FF
				|| codePoint is >= 0xE000 and <= 0xFFFD;

			StringHelper.RemoveInvalidXmlChars(input).Should().Be(valid ? input : "ab", "U+{0:X4}", codePoint);
		}
	}

	[DataTestMethod]
	[DataRow("FFFF0009")]
	[DataRow("FFFF0041")]
	[DataRow("80000041")]
	[DataRow("FFFFFFFF")]
	[DataRow("100000000")]
	[DataRow("7FFFFFFF")]
	[DataRow("110000")]
	[DataRow("D800")]
	[DataRow("DC00")]
	[DataRow("FFFE")]
	[DataRow("FFFF")]
	public void CleanInvalidXmlChars_RemovesInvalidCodePointReferences(string hex)
	{
		StringHelper.CleanInvalidXmlChars($"a&#x{hex};b").Should().Be("ab");
	}

	[TestMethod]
	public void CleanInvalidXmlChars_AllC0References()
	{
		for (var codePoint = 0; codePoint < 0x20; codePoint++)
		{
			var reference = $"&#x{codePoint:X};";
			var expected = codePoint is 0x9 or 0xA or 0xD ? reference : string.Empty;

			StringHelper.CleanInvalidXmlChars($"a{reference}b").Should().Be($"a{expected}b", "U+{0:X4}", codePoint);
		}
	}

	[DataTestMethod]
	[DataRow(0x10000)]
	[DataRow(0x1F600)]
	[DataRow(0x20000)]
	[DataRow(0x10FFFF)]
	public void CleanInvalidXmlChars_KeepsSupplementaryUnicode(int codePoint)
	{
		var value = char.ConvertFromUtf32(codePoint);
		var input = $"a{value}b";
		var reference = $"a&#x{codePoint:X};b";

		StringHelper.RemoveInvalidXmlChars(input).Should().Be(input);
		StringHelper.CleanInvalidXmlChars(input).Should().Be(input);
		StringHelper.CleanInvalidXmlChars(reference).Should().Be(reference);
		StringHelper.RemoveInvalidXmlChars($"\u0008{value}\u001F").Should().Be(value);
	}

	[TestMethod]
	public void RemoveInvalidXmlChars_RemovesMalformedSurrogates()
	{
		var inputs = new[]
		{
			"a\uD800b",
			"a\uDC00b",
			"a\uD800\u0008\uDC00b",
			"a\uDC00\uD800b",
		};

		foreach (var input in inputs)
		{
			StringHelper.RemoveInvalidXmlChars(input).Should().Be("ab");
			StringHelper.CleanInvalidXmlChars(input).Should().Be("ab");
		}
	}

	[TestMethod]
	public void RemoveInvalidXmlChars_KeepsValidPairsNextToMalformedSurrogates()
	{
		var input = "\uD800\U0001F600\uDC00\u0008\U00020000\uD800";

		StringHelper.RemoveInvalidXmlChars(input).Should().Be("\U0001F600\U00020000");
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
