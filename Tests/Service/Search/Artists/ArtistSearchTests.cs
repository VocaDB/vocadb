#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Search.Artists;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Search.Artists
{
	/// <summary>
	/// Tests for <see cref="ArtistSearch"/>.
	/// </summary>
	[TestClass]
	public class ArtistSearchTests
	{
		private readonly ArtistSearch _artistSearch;
		private readonly FakeArtistRepository _db = new();

		public ArtistSearchTests()
		{
			_artistSearch = new ArtistSearch(ContentLanguagePreference.Default, _db.CreateContext(), new EntryUrlParser());

			var artist1 = _db.Save(CreateEntry.Artist(ArtistType.Producer, name: "XenonP"));
			_db.SaveNames(artist1);
			_db.Save(artist1.CreateWebLink("Twitter", "https://twitter.com/XenonP_XM", WebLinkCategory.Official, disabled: false));
			_db.SaveNames(_db.Save(CreateEntry.Artist(ArtistType.Producer, name: "Clean Tears")));
			_db.SaveNames(_db.Save(CreateEntry.Artist(ArtistType.Vocaloid, name: "Hatsune Miku")));
		}

		[TestMethod]
		public void Find()
		{
			var result = _artistSearch.Find(new ArtistQueryParams
			{
				Common = new()
				{
					TextQuery = ArtistSearchTextQuery.Create("XenonP")
				}
			});

			result.Items.Length.Should().Be(1, "Got 1 result");
			result.Items[0].DefaultName.Should().Be("XenonP", "Result as expected");
		}

		[TestMethod]
		public void Find_ByTwitter()
		{
			var result = _artistSearch.Find(new ArtistQueryParams
			{
				Common = new()
				{
					TextQuery = ArtistSearchTextQuery.Create("https://twitter.com/XenonP_XM")
				}
			});

			result.Items.Length.Should().Be(1, "Got 1 result");
			result.Items[0].DefaultName.Should().Be("XenonP", "Result as expected");
		}
		
		[TestMethod]
		public void Find_ByTwitter_WithShortcut()
		{
			var result = _artistSearch.Find(new ArtistQueryParams
			{
				Common = new()
				{
					TextQuery = ArtistSearchTextQuery.Create("t/XenonP_XM")
				}
			});

			result.Items.Length.Should().Be(1, "Got 1 result");
			result.Items[0].DefaultName.Should().Be("XenonP", "Result as expected");
		}

		[TestMethod]
		public void Find_ByTwitter_EndsWithP()
		{
			var artist = _db.Save(CreateEntry.Artist(ArtistType.Producer, name: "Uji"));
			_db.SaveNames(artist);
			_db.Save(artist.CreateWebLink("Twitter", "https://twitter.com/Uji_RaychoruiP", WebLinkCategory.Official, disabled: false));

			var result = _artistSearch.Find(new ArtistQueryParams
			{
				Common = new()
				{
					TextQuery = ArtistSearchTextQuery.Create("https://twitter.com/Uji_RaychoruiP")
				}
			});

			result.Items.Length.Should().Be(1, "Got 1 result");
			result.Items[0].DefaultName.Should().Be("Uji", "Result as expected");
		}
	}
}
