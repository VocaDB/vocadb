#nullable disable

using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.Globalization;

/// <summary>
/// Tests for <see cref="EnglishTranslatedString"/>.
/// </summary>
[TestClass]
public class EnglishTranslatedStringTests
{
	[TestMethod]
	public void Properties_RemoveInvalidXmlChars()
	{
		var text = new EnglishTranslatedString("初\u0008音", "Mi\u000Bku");

		text.Original.Should().Be("初音");
		text.English.Should().Be("Miku");

		text.Original = "a\uD800b\uFFFE";
		text.English = "c\uDC00d\uFFFF";

		text.Original.Should().Be("ab");
		text.English.Should().Be("cd");
	}

	[TestMethod]
	public void Properties_KeepValidWhitespaceAndUnicode()
	{
		var value = "初音ミク\t\n\r\U0001F600\U00020000\u0085";
		var text = new EnglishTranslatedString(value, value);

		text.Original.Should().Be(value);
		text.English.Should().Be(value);
	}

	[TestMethod]
	public void Properties_AllowEmptyAndControlOnlyText()
	{
		var text = new EnglishTranslatedString();

		text.Original.Should().BeEmpty();
		text.English.Should().BeEmpty();

		text.Original = "\u0008";
		text.English = "\u000B\u001F";

		text.IsEmpty.Should().BeTrue();
		text.HasEnglish.Should().BeFalse();
	}

	[TestMethod]
	public void Properties_RejectNull()
	{
		var text = new EnglishTranslatedString();

		Invoking(() => text.Original = null).Should().Throw<ArgumentNullException>();
		Invoking(() => text.English = null).Should().Throw<ArgumentNullException>();
	}

	[TestMethod]
	public void DescriptionsAndNotes_AreCleanedBeforeArchiving()
	{
		var artist = new Artist();
		var song = new Song();

		artist.Description.Original = "初\u0008音";
		artist.Description.English = "Mi\u000Bku";
		song.Notes.Original = "原\u0008曲";
		song.Notes.English = "Ori\u001Fginal";

		artist.Description.Original.Should().Be("初音");
		artist.Description.English.Should().Be("Miku");
		song.Notes.Original.Should().Be("原曲");
		song.Notes.English.Should().Be("Original");
	}

	[TestMethod]
	public void CopyFrom_CleansBeforeTrimmingAndComparing()
	{
		var text = new EnglishTranslatedString("初音", "Miku");
		var contract = new EnglishTranslatedStringContract
		{
			Original = "\u0008 初音 \u0008",
			English = "\u0008 Miku \u0008",
		};

		var changed = text.CopyFrom(contract);

		changed.Should().BeFalse();
		text.Original.Should().Be("初音");
		text.English.Should().Be("Miku");
		contract.English.Should().Be("\u0008 Miku \u0008");
	}

	[TestMethod]
	public void CopyFrom_RemovesInvalidXmlChars()
	{
		var text = new EnglishTranslatedString();
		var contract = new EnglishTranslatedStringContract { Original = "初\u0008音", English = "Mi\u000Bku" };

		var changed = text.CopyFrom(contract);

		changed.Should().BeTrue();
		text.Original.Should().Be("初音");
		text.English.Should().Be("Miku");
	}

	[TestMethod]
	public void CopyFrom_ControlOnlyTextIsUnchanged()
	{
		var text = new EnglishTranslatedString();
		var contract = new EnglishTranslatedStringContract { Original = "\u0008", English = "\u000B" };

		text.CopyFrom(contract).Should().BeFalse();
	}

	[TestMethod]
	public void CopyFrom_Changed()
	{
		var source = new EnglishTranslatedString("ミク", "Miku");
		var target = new EnglishTranslatedStringContract { Original = "ミクさんマジ天使", English = "Hatsune Miku is truly my angel" };

		var changed = source.CopyFrom(target);

		changed.Should().BeTrue("changed");
		source.Original.Should().Be("ミクさんマジ天使", "Original");
	}

	[TestMethod]
	public void CopyFrom_Trim()
	{
		var source = new EnglishTranslatedString();
		var target = new EnglishTranslatedStringContract { Original = " " };

		var changed = source.CopyFrom(target);

		changed.Should().BeFalse("changed");
		source.Original.Should().Be(string.Empty, "Original");
	}
}
