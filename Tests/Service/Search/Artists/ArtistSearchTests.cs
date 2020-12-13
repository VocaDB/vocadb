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
		private readonly ArtistSearch artistSearch;
		private readonly FakeArtistRepository db = new FakeArtistRepository();

		public ArtistSearchTests()
		{
			artistSearch = new ArtistSearch(ContentLanguagePreference.Default, db.CreateContext(), new EntryUrlParser());

			var artist1 = db.Save(CreateEntry.Artist(ArtistType.Producer, name: "XenonP"));
			db.SaveNames(artist1);
			db.Save(artist1.CreateWebLink("Twitter", "https://twitter.com/XenonP_XM", WebLinkCategory.Official, disabled: false));
			db.SaveNames(db.Save(CreateEntry.Artist(ArtistType.Producer, name: "Clean Tears")));
			db.SaveNames(db.Save(CreateEntry.Artist(ArtistType.Vocaloid, name: "Hatsune Miku")));
		}

		[TestMethod]
		public void Find()
		{
			var result = artistSearch.Find(new ArtistQueryParams
			{
				Common = {
					TextQuery = ArtistSearchTextQuery.Create("XenonP")
				}
			});

			Assert.AreEqual(1, result.Items.Length, "Got 1 result");
			Assert.AreEqual("XenonP", result.Items[0].DefaultName, "Result as expected");
		}

		[TestMethod]
		public void Find_ByTwitter()
		{
			var result = artistSearch.Find(new ArtistQueryParams
			{
				Common = {
					TextQuery = ArtistSearchTextQuery.Create("https://twitter.com/XenonP_XM")
				}
			});

			Assert.AreEqual(1, result.Items.Length, "Got 1 result");
			Assert.AreEqual("XenonP", result.Items[0].DefaultName, "Result as expected");
		}

		[TestMethod]
		public void Find_ByTwitter_EndsWithP()
		{
			var artist = db.Save(CreateEntry.Artist(ArtistType.Producer, name: "Uji"));
			db.SaveNames(artist);
			db.Save(artist.CreateWebLink("Twitter", "https://twitter.com/Uji_RaychoruiP", WebLinkCategory.Official, disabled: false));

			var result = artistSearch.Find(new ArtistQueryParams
			{
				Common = {
					TextQuery = ArtistSearchTextQuery.Create("https://twitter.com/Uji_RaychoruiP")
				}
			});

			Assert.AreEqual(1, result.Items.Length, "Got 1 result");
			Assert.AreEqual("Uji", result.Items[0].DefaultName, "Result as expected");
		}
	}
}
