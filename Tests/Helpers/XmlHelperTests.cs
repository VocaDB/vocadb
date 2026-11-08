using System.Diagnostics.CodeAnalysis;
using System.Xml.Linq;
using VocaDb.Model.DataContracts.Albums;
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
		var album = new ArchivedAlbumContract { Description = "Miku\x00\x01\x08\x0B\x0C\x0E\x1FMiku" };

		var res = SerializeToObjectAndBack(album);

		res.Description.Should().Be("MikuMiku", "all invalid characters are removed");
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
