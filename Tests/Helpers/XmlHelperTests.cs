using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers;

/// <summary>
/// Tests for <see cref="XmlHelper"/>.
/// </summary>
[TestClass]
public class XmlHelperTests
{
	[return: NotNullIfNotNull(nameof(obj))]
	private static T? SerializeToObjectAndBack<T>(T? obj)
	{
		var xml = XmlHelper.SerializeToXml(obj);

		xml.Should().NotBeNull("result is not null");

		return XmlHelper.DeserializeFromXml<T>(xml);
	}

	[TestMethod]
	public void SerializeToUTF8XmlString_ValidObject()
	{
		var album = new ArchivedAlbumContract { Description = "Miku Miku!" };
		var doc = XmlHelper.SerializeToXml(album);
		var declaration = new XDeclaration("1.0", "utf-8", "yes");
		var reference = declaration + Environment.NewLine + doc;

		var res = XmlHelper.SerializeToUTF8XmlString(album);

		res.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>").Should().BeTrue("Header is correct");
		res.Should().Be(reference, "Result as expected");
	}

	[TestMethod]
	public void SerializeToUTF8XmlString_ValidDocument()
	{
		var doc = new XDocument(new XElement("root", new XElement("MikuMiku")));
		var declaration = new XDeclaration("1.0", "utf-8", "yes");
		var reference = declaration + Environment.NewLine + doc;

		var res = XmlHelper.SerializeToUTF8XmlString(doc);

		res.StartsWith("<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\"?>").Should().BeTrue("Header is correct");
		res.Should().Be(reference, "Result as expected");
	}

	[TestMethod]
	public void SerializeToXml_ValidObject()
	{
		var album = new ArchivedAlbumContract { Description = "Miku Miku!" };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be(album.Description, "string is intact");
	}

	[TestMethod]
	public void SerializeToXml_Unicode()
	{
		var album = new ArchivedAlbumContract { Description = "初音ミク" };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be(album.Description, "string is intact");
	}

	[TestMethod]
	public void SerializeToXml_ForbiddenChars()
	{
		var name = "Miku Miku!";
		var album = new ArchivedAlbumContract { Description = name + '\x02' };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be(name, "invalid character is removed");
	}

	[TestMethod]
	public void SerializeToXml_MultipleForbiddenChars()
	{
		var name = "Miku Miku!";
		var album = new ArchivedAlbumContract { Description = $"\x01{name}\x02" };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be(name, "invalid characters are removed");
	}

	[TestMethod]
	public void SerializeToXml_AllForbiddenChars()
	{
		var forbidden = new string(Enumerable.Range(0, 0x20)
			.Where(c => c is not (0x9 or 0xA or 0xD))
			.Select(c => (char)c)
			.ToArray());
		var album = new ArchivedAlbumContract { Description = $"Miku{forbidden}\uFFFE\uFFFFMiku" };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be("MikuMiku", "all invalid characters are removed");
	}

	[TestMethod]
	public void SerializeToXml_AllC0Characters()
	{
		for (var codePoint = 0; codePoint < 0x20; codePoint++)
		{
			var description = $"a{(char)codePoint}b";
			var album = new ArchivedAlbumContract { Description = description };

			var result = SerializeToObjectAndBack(album);

			result.Description.Should().Be(codePoint is 0x9 or 0xA or 0xD ? description : "ab", "U+{0:X4}", codePoint);
		}
	}

	[TestMethod]
	public void SerializeToXml_KeepsC1Characters()
	{
		var description = new string(Enumerable.Range(0x7F, 0x21).Select(c => (char)c).ToArray());
		var album = new ArchivedAlbumContract { Description = description };

		var result = SerializeToObjectAndBack(album);

		result.Description.Should().Be(description);
	}

	[DataTestMethod]
	[DataRow(0x10000)]
	[DataRow(0x1F600)]
	[DataRow(0x20000)]
	[DataRow(0x10FFFF)]
	public void SerializeToXml_KeepsSupplementaryUnicode(int codePoint)
	{
		var description = $"a{char.ConvertFromUtf32(codePoint)}b";
		var album = new ArchivedAlbumContract { Description = description };

		var result = SerializeToObjectAndBack(album);

		result.Description.Should().Be(description);
	}

	[TestMethod]
	public void SerializeToXml_KeepsLiteralCharacterReferences()
	{
		var description = "&#x8; &#xFFFF0009; &#8;";
		var album = new ArchivedAlbumContract { Description = description };

		var result = SerializeToObjectAndBack(album);

		result.Description.Should().Be(description);
	}

	[TestMethod]
	public void SerializeToXml_CleanedMalformedUnicode()
	{
		var text = new EnglishTranslatedString("a\uD800b\u0008\U0001F600", "c\uDC00d\U00020000");

		var result = SerializeToObjectAndBack(text);

		result.Original.Should().Be("ab\U0001F600");
		result.English.Should().Be("cd\U00020000");
	}

	[TestMethod]
	public void SerializeToXml_RejectsRawMalformedSurrogates()
	{
		foreach (var invalid in new[] { "\uD800", "\uDC00", "\uD800\u0008\uDC00", "\uDC00\uD800" })
		{
			var album = new ArchivedAlbumContract { Description = $"a{invalid}b" };

			Invoking(() => SerializeToObjectAndBack(album)).Should().Throw<InvalidOperationException>()
				.WithInnerException<ArgumentException>();
		}
	}

	[TestMethod]
	public void SerializeToXml_ValidWhitespace()
	{
		var description = "Miku\tMiku\nMiku\rMiku";
		var album = new ArchivedAlbumContract { Description = description };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be(description, "valid whitespace is preserved");
	}
}
