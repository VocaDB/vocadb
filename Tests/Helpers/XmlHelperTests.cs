#nullable disable

using System;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{
	/// <summary>
	/// Tests for <see cref="XmlHelper"/>.
	/// </summary>
	[TestClass]
	public class XmlHelperTests
	{
		private static T SerializeToObjectAndBack<T>(T obj)
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

			album.Description.IsNormalized().Should().BeTrue();

			this.Invoking(_ => SerializeToObjectAndBack(album)).Should().Throw<InvalidOperationException>();
		}

		[TestMethod]
		public void SerializeToXml_MultipleForbiddenChars()
		{
			var name = "Miku Miku!";
			var album = new ArchivedAlbumContract { Description = $"\x01{name}\x02" };

			album.Description.IsNormalized().Should().BeTrue();

			this.Invoking(_ => SerializeToObjectAndBack(album)).Should().Throw<InvalidOperationException>();
		}
	}
}
