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
