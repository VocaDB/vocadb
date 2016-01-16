using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Domain.Songs {

	/// <summary>
	/// Tests for <see cref="Song"/>.
	/// </summary>
	[TestClass]
	public class SongTests {

		private Artist artist;
		private LyricsForSong lyrics;
		private Song song;

		private PVForSong CreatePV(PVService service = PVService.Youtube, PVType pvType = PVType.Original, DateTime? publishDate = null) {
			return song.CreatePV(new PVContract { Service = service, PVId = "test", Name = "test", PublishDate = publishDate, PVType = pvType });
		}

		private void TestUpdatePublishDateFromPVs(DateTime? expected, params PVForSong[] pvs) {

			song.UpdatePublishDateFromPVs();

			Assert.AreEqual(expected, song.PublishDate.DateTime, "PublishDate");

		}

		[TestInitialize]
		public void Setup() {

			artist = new Artist(TranslatedString.Create("Minato")) { Id = 1, ArtistType = ArtistType.Producer };
			song = new Song(new LocalizedString("Soar", ContentLanguageSelection.English));
			lyrics = song.CreateLyrics(ContentLanguageSelection.Japanese, "Miku!", "miku");

		}

		[TestMethod]
		public void Ctor_LocalizedString() {

			song = new Song(new LocalizedString("song", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, song.Names.Count(), "Names count");
			Assert.IsTrue(song.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(song.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("song", song.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");

		}

		[TestMethod]
		public void AddTag_NewTag() {

			var tag = CreateEntry.Tag("rock");
			var result = song.AddTag(tag);

			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result.IsNew, "Is new");
			Assert.AreEqual(tag, result.Result.Tag, "Added tag");
			Assert.AreEqual(1, song.Tags.Usages.Count, "Number of tag usages for song");
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");

		}

		[TestMethod]
		public void AddTag_ExistingTag() {

			var tag = CreateEntry.Tag("rock");
			song.AddTag(tag);
			var result = song.AddTag(tag);

			Assert.IsNotNull(result, "result");
			Assert.IsFalse(result.IsNew, "Is new");
			Assert.AreEqual(tag, result.Result.Tag, "Added tag");
			Assert.AreEqual(1, song.Tags.Usages.Count, "Number of tag usages for song");
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");

		}

		[TestMethod]
		public void LyricsFromParents_NoLyrics() {

			var result = new Song().GetLyricsFromParents(0);

			Assert.AreEqual(0, result.Count, "no lyrics");

		}

		[TestMethod]
		public void LyricsFromParents_NoParent() {

			var result = song.GetLyricsFromParents(0);

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(lyrics, result.First(), "returned lyrics from entry");

		}

		[TestMethod]
		public void LyricsFromParents_FromParent() {

			var derived = new Song();
			derived.OriginalVersion = song;
			var result = derived.GetLyricsFromParents(0);

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(lyrics, result.First(), "returned lyrics from entry");

		}

		[TestMethod]
		public void SyncArtists_Duplicate() {
			
			var newArtists = new[] {
				new ArtistForSongContract(new ArtistForSong(song, artist, false, ArtistRoles.Default), ContentLanguagePreference.Default),
				new ArtistForSongContract(new ArtistForSong(song, artist, false, ArtistRoles.Composer), ContentLanguagePreference.Default),
			};

			song.SyncArtists(newArtists, c => c.Artist.Id == artist.Id ? artist : null);

			Assert.AreEqual(1, song.AllArtists.Count, "Only one artist");
			Assert.AreEqual(artist, song.AllArtists.First().Artist, "Artist is as expected");

		}

		/// <summary>
		/// Extra artists (just name, no entry) will not be removed when syncing with real artists.
		/// </summary>
		[TestMethod]
		public void SyncArtists_WillNotRemoveExtraArtists() {

			var link = song.AddArtist("Extra artist", false, ArtistRoles.Composer);
			var newArtists = new[] { new ArtistContract(artist, ContentLanguagePreference.Default) };

			song.SyncArtists(newArtists, ac => new[] { artist });

			Assert.AreEqual(2, song.AllArtists.Count, "artists count");
			Assert.IsTrue(song.HasArtistLink(link), "Still has the extra artist");

		}

		[TestMethod]
		public void UpdatePVServices_None() {

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Nothing, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_One() {

			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.Youtube, song.PVServices);

		}

		[TestMethod]
		public void UpdatePVServices_Multiple() {

			CreatePV(PVService.NicoNicoDouga);
			CreatePV(PVService.SoundCloud);
			CreatePV(PVService.Youtube);

			song.UpdatePVServices();

			Assert.AreEqual(PVServices.NicoNicoDouga | PVServices.SoundCloud | PVServices.Youtube, song.PVServices);

		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_NoPVs_NotUpdated() {
			
			TestUpdatePublishDateFromPVs(null);

		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_HasPV_Updated() {

			TestUpdatePublishDateFromPVs(new DateTime(2010, 1, 1), CreatePV(publishDate: new DateTime(2010, 1, 1)));

		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_HasPV_NotOriginal_NotUpdated() {

			TestUpdatePublishDateFromPVs(null, CreatePV(publishDate: new DateTime(2010, 1, 1), pvType: PVType.Reprint));

		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_MultiplePVs() {

			TestUpdatePublishDateFromPVs(new DateTime(2008, 6, 12), 
				CreatePV(publishDate: new DateTime(2010, 1, 1)),
				CreatePV(publishDate: new DateTime(2008, 6, 12))
			);

		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_AlbumReleaseDate() {

			var album = CreateEntry.Album();
			album.OriginalReleaseDate.Year = 2007;
			album.OriginalReleaseDate.Month = 6;
			album.OriginalReleaseDate.Day = 1;

			album.AddSong(song, 1, 1);

			TestUpdatePublishDateFromPVs(new DateTime(2007, 6, 1),
				CreatePV(publishDate: new DateTime(2010, 1, 1))
			);

		}

	}

}
