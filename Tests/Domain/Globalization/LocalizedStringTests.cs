using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Venues;

namespace VocaDb.Tests.Domain.Globalization;

[TestClass]
public class LocalizedStringTests
{
	private static INameFactory<LocalizedStringWithId>[] NameFactories()
	{
		return new INameFactory<LocalizedStringWithId>[]
		{
			new Album(),
			new Artist(),
			new ReleaseEvent(),
			new ReleaseEventSeries(),
			new Song(),
			new Tag(),
			new Venue(),
		};
	}

	[TestMethod]
	public void Value_EmptyDefaultsRemainSupported()
	{
		new LocalizedString().Value.Should().BeEmpty();
		new LocalizedString(string.Empty, ContentLanguageSelection.English).Value.Should().BeEmpty();
		new LocalizedStringWithId().SearchValue.Should().BeEmpty();
	}

	[TestMethod]
	public void ValueWithId_CleansSearchValue()
	{
		var str = new LocalizedStringWithId("Miku\u0008Miku", ContentLanguageSelection.English);

		str.Value.Should().Be("MikuMiku");
		str.SearchValue.Should().Be("MIKUMIKU");

		str.Value = "Miku\u000B\u0008Miku!";

		str.Value.Should().Be("MikuMiku!");
		str.SearchValue.Should().Be("MIKUMIKU!");
	}

	[DataTestMethod]
	[DataRow("\u0008")]
	[DataRow("\u000B\u000C")]
	[DataRow("\uFFFE\uFFFF")]
	public void CreateName_RejectsControlOnlyNames(string value)
	{
		foreach (var factory in NameFactories())
			Invoking(() => factory.CreateName(value, ContentLanguageSelection.English)).Should().Throw<ArgumentException>();
	}

	[TestMethod]
	public void CreateName_LocalizedControlOnlyNameRejected()
	{
		var name = new LocalizedString("\u0008", ContentLanguageSelection.English);

		Invoking(() => new Song().CreateName(name)).Should().Throw<ArgumentException>();
		Invoking(() => new Tag().CreateName(name)).Should().Throw<ArgumentException>();
	}

	[TestMethod]
	public void CreateName_CleansLocalizedContract()
	{
		var contract = new LocalizedStringContract("Foo\u000BBar", ContentLanguageSelection.English);

		var name = new ReleaseEvent().CreateName(contract);

		name.Value.Should().Be("FooBar");
		name.SearchValue.Should().Be("FOOBAR");
	}

	[DataTestMethod]
	[DataRow(" ")]
	[DataRow("\t\n\r")]
	public void CreateName_KeepsWhitespaceNames(string value)
	{
		foreach (var factory in NameFactories())
			factory.CreateName(value, ContentLanguageSelection.English).Value.Should().Be(value);
	}

	[TestMethod]
	public void TranslatedNames_KeepEmptyTranslations()
	{
		var translated = new TranslatedString(string.Empty, string.Empty, "Miku");

		new Album(translated).Names.Names.Should().HaveCount(3);
		new Artist(translated).Names.Names.Should().HaveCount(3);
		new Song(translated).Names.Names.Should().HaveCount(3);
	}

	[TestMethod]
	public void Value_RemovesInvalidXmlChars()
	{
		var str = new LocalizedString("初音\x08ミク", ContentLanguageSelection.Japanese);

		str.Value.Should().Be("初音ミク");
	}

	[TestMethod]
	public void Value_KeepsValidWhitespace()
	{
		var str = new LocalizedString("a\tb\nc\rd", ContentLanguageSelection.Japanese);

		str.Value.Should().Be("a\tb\nc\rd");
	}
}
