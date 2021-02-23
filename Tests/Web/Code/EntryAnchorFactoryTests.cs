#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Web.Code;

namespace VocaDb.Tests.Web.Code
{
	/// <summary>
	/// Tests for <see cref="EntryAnchorFactory"/>.
	/// </summary>
	[TestClass]
	public class EntryAnchorFactoryTests
	{
		private EntryAnchorFactory _factory;

		[TestInitialize]
		public void SetUp()
		{
			_factory = new EntryAnchorFactory("http://test.vocadb.net");
		}

		[TestMethod]
		public void CreateEntryLink_Components()
		{
			var result = _factory.CreateEntryLink(EntryType.Artist, 39, "Hatsune Miku");

			result.Should().Be("<a href=\"/Ar/39\">Hatsune Miku</a>", "result");
		}

		[TestMethod]
		public void CreateEntryLink_Entry()
		{
			var artist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39 };
			var result = _factory.CreateEntryLink(artist);

			result.Should().Be("<a href=\"/Ar/39\">Hatsune Miku</a>", "result");
		}

		[TestMethod]
		public void CreateEntryLink_HtmlEncode()
		{
			var song = new Song(TranslatedString.Create("Sentaku <love or dead>")) { Id = 39 };
			var result = _factory.CreateEntryLink(song);

			result.Should().Be("<a href=\"/S/39\">Sentaku &lt;love or dead&gt;</a>", "result");
		}
	}
}
