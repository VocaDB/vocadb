using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Tests.Domain.Globalization;

[TestClass]
public class LocalizedStringTests
{
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
