#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Service;
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestData;

namespace VocaDb.Tests.Domain.Songs
{
	/// <summary>
	/// Tests for <see cref="Song"/>.
	/// </summary>
	[TestClass]
	public class SongTests
	{
		private class SpecialTags : ISpecialTags
		{
			public int ChangedLyrics { get; set; }
		}

		private class EntryTypeTags : IEntryTypeTagRepository
		{
			public int Cover { get; set; }
			public int Instrumental { get; set; }
			public int Remix { get; set; }
			public int SongTypeTagId(SongType songType) => 0;
			public Tag GetTag<TSubType>(EntryType entryType, TSubType subType) where TSubType : Enum => null;
			public Tag GetTag(EntryTypeAndSubType fullEntryType) => null;
		}

		private Artist _artist;
		private readonly List<Artist> _artists = new();
		private Tag _changedLyricsTag;
		private EntryTypeTags _entryTypeTags;
		private Tag _instrumentalTag;
		private LyricsForSong _lyrics;
		private Song _song;
		private SpecialTags _specialTags;
		private Artist _vocalist;

		private Func<ArtistForSongContract, Artist> _artistFunc;

		private IList<LyricsForSong> CallGetLyricsFromParents(Song song)
		{
			return song.GetLyricsFromParents(_specialTags, _entryTypeTags);
		}

		private PVForSong CreatePV(PVService service = PVService.Youtube, PVType pvType = PVType.Original, DateTime? publishDate = null)
		{
			return _song.CreatePV(new PVContract { Service = service, PVId = "test", Name = "test", PublishDate = publishDate, PVType = pvType });
		}

		private void TestUpdatePublishDateFromPVs(DateTime? expected, params PVForSong[] pvs)
		{
			_song.UpdatePublishDateFromPVs();

			Assert.AreEqual(expected, _song.PublishDate.DateTime, "PublishDate");
		}

		[TestInitialize]
		public void Setup()
		{
			_artist = new Artist(TranslatedString.Create("Minato")) { Id = 1, ArtistType = ArtistType.Producer };
			_vocalist = CreateEntry.Vocalist(39);
			_artists.Add(_artist);
			_artists.Add(_vocalist);
			_song = new Song(new LocalizedString("Soar", ContentLanguageSelection.English));
			_lyrics = _song.CreateLyrics("Miku!", "miku", string.Empty, TranslationType.Original, "ja");
			_instrumentalTag = CreateEntry.Tag("instrumental", 1);
			_changedLyricsTag = CreateEntry.Tag("changed lyrics", 2);
			_specialTags = new SpecialTags
			{
				ChangedLyrics = _changedLyricsTag.Id
			};
			_entryTypeTags = new EntryTypeTags
			{
				Instrumental = _instrumentalTag.Id,
			};
			_artistFunc = (contract => _artists.FirstOrDefault(a => a.Id == contract.Artist?.Id));
		}

		[TestMethod]
		public void Ctor_LocalizedString()
		{
			_song = new Song(new LocalizedString("song", ContentLanguageSelection.Romaji));

			Assert.AreEqual(1, _song.Names.Count(), "Names count");
			Assert.IsTrue(_song.Names.HasNameForLanguage(ContentLanguageSelection.Romaji), "Has name for Romaji");
			Assert.IsFalse(_song.Names.HasNameForLanguage(ContentLanguageSelection.English), "Does not have name for English");
			Assert.AreEqual("song", _song.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName, "Display name");
		}

		[TestMethod]
		public void AddTag_NewTag()
		{
			var tag = CreateEntry.Tag("rock");
			var result = _song.AddTag(tag);

			Assert.IsNotNull(result, "result");
			Assert.IsTrue(result.IsNew, "Is new");
			Assert.AreEqual(tag, result.Result.Tag, "Added tag");
			Assert.AreEqual(1, _song.Tags.Usages.Count, "Number of tag usages for song");
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");
		}

		[TestMethod]
		public void AddTag_ExistingTag()
		{
			var tag = CreateEntry.Tag("rock");
			_song.AddTag(tag);
			var result = _song.AddTag(tag);

			Assert.IsNotNull(result, "result");
			Assert.IsFalse(result.IsNew, "Is new");
			Assert.AreEqual(tag, result.Result.Tag, "Added tag");
			Assert.AreEqual(1, _song.Tags.Usages.Count, "Number of tag usages for song");
			Assert.AreEqual(1, tag.UsageCount, "Number of usages for tag");
		}

		[TestMethod]
		public void LyricsFromParents_NoLyrics()
		{
			var result = CallGetLyricsFromParents(new Song());

			Assert.AreEqual(0, result.Count, "no lyrics");
		}

		[TestMethod]
		public void LyricsFromParents_NoParent()
		{
			var result = CallGetLyricsFromParents(_song);

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(_lyrics, result.First(), "returned lyrics from entry");
		}

		[TestMethod]
		public void LyricsFromParents_Instrumental()
		{
			var derived = new Song
			{
				SongType = SongType.Instrumental,
				OriginalVersion = _song
			};

			var result = CallGetLyricsFromParents(derived);
			Assert.AreEqual(0, result.Count, "No lyrics inherited for instrumental");
		}

		[TestMethod]
		public void LyricsFromParents_FromParent()
		{
			var derived = new Song
			{
				OriginalVersion = _song
			};
			var result = CallGetLyricsFromParents(derived);

			Assert.AreEqual(1, result.Count, "one entry");
			Assert.AreSame(_lyrics, result.First(), "returned lyrics from entry");
		}

		[TestMethod]
		public void LyricsFromParents_TwoLevelsWithInstrumental()
		{
			// original -> instrumental -> derived
			var instrumental = new Song
			{
				SongType = SongType.Instrumental,
				OriginalVersion = _song
			};

			var derived = new Song
			{
				OriginalVersion = instrumental
			};

			var result = CallGetLyricsFromParents(derived);
			Assert.AreSame(_lyrics, result.FirstOrDefault(), "returned lyrics from entry");
		}

		[TestMethod]
		public void SyncArtists_Duplicate()
		{
			var newArtists = new[] {
				new ArtistForSongContract(new ArtistForSong(_song, _artist, false, ArtistRoles.Default), ContentLanguagePreference.Default),
				new ArtistForSongContract(new ArtistForSong(_song, _artist, false, ArtistRoles.Composer), ContentLanguagePreference.Default),
			};

			_song.SyncArtists(newArtists, c => c.Artist.Id == _artist.Id ? _artist : null);

			Assert.AreEqual(1, _song.AllArtists.Count, "Only one artist");
			Assert.AreEqual(_artist, _song.AllArtists.First().Artist, "Artist is as expected");
		}

		/// <summary>
		/// Extra artists (just name, no entry) will not be removed when syncing with real artists.
		/// </summary>
		[TestMethod]
		public async Task SyncArtists_WillNotRemoveExtraArtists()
		{
			var link = _song.AddArtist("Extra artist", false, ArtistRoles.Composer);
			var newArtists = new[] { new ArtistContract(_artist, ContentLanguagePreference.Default) };

			await _song.SyncArtistsAsync(newArtists, ac => Task.FromResult(new List<Artist> { _artist }));

			Assert.AreEqual(2, _song.AllArtists.Count, "artists count");
			Assert.IsTrue(_song.HasArtistLink(link), "Still has the extra artist");
		}

		[TestMethod]
		public void SyncArtists_CustomName()
		{
			var contract = new ArtistForSongContract
			{
				Artist = new ArtistContract(_artist, ContentLanguagePreference.Default),
				Name = "RyuuseiP",
				IsCustomName = true
			};

			_song.SyncArtists(new[] { contract, new ArtistForSongContract(new ArtistContract(_vocalist, ContentLanguagePreference.Default)) }, _artistFunc);

			Assert.AreEqual(2, _song.Artists.Count(), "Number of artists");
			var producerLink = _song.Artists.FirstOrDefault(a => a.Artist?.Id == _artist.Id);
			Assert.IsNotNull(producerLink, "Artist link was added");
			Assert.AreEqual("RyuuseiP", producerLink.Name, "Added link name");
			Assert.AreEqual("RyuuseiP feat. Hatsune Miku", _song.ArtistString.Default, "Artist string");
		}

		[TestMethod]
		public void UpdatePVServices_None()
		{
			_song.UpdatePVServices();

			Assert.AreEqual(PVServices.Nothing, _song.PVServices);
		}

		[TestMethod]
		public void UpdatePVServices_One()
		{
			CreatePV(PVService.Youtube);

			_song.UpdatePVServices();

			Assert.AreEqual(PVServices.Youtube, _song.PVServices);
		}

		[TestMethod]
		public void UpdatePVServices_Multiple()
		{
			CreatePV(PVService.NicoNicoDouga);
			CreatePV(PVService.SoundCloud);
			CreatePV(PVService.Youtube);

			_song.UpdatePVServices();

			Assert.AreEqual(PVServices.NicoNicoDouga | PVServices.SoundCloud | PVServices.Youtube, _song.PVServices);
		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_NoPVs_NotUpdated()
		{
			TestUpdatePublishDateFromPVs(null);
		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_HasPV_Updated()
		{
			TestUpdatePublishDateFromPVs(new DateTime(2010, 1, 1), CreatePV(publishDate: new DateTime(2010, 1, 1)));
		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_HasPV_NotOriginal_NotUpdated()
		{
			TestUpdatePublishDateFromPVs(null, CreatePV(publishDate: new DateTime(2010, 1, 1), pvType: PVType.Reprint));
		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_MultiplePVs()
		{
			TestUpdatePublishDateFromPVs(new DateTime(2008, 6, 12),
				CreatePV(publishDate: new DateTime(2010, 1, 1)),
				CreatePV(publishDate: new DateTime(2008, 6, 12))
			);
		}

		[TestMethod]
		public void UpdatePublishDateFromPVs_AlbumReleaseDate()
		{
			var album = CreateEntry.Album();
			album.OriginalReleaseDate.Year = 2007;
			album.OriginalReleaseDate.Month = 6;
			album.OriginalReleaseDate.Day = 1;

			album.AddSong(_song, 1, 1);

			TestUpdatePublishDateFromPVs(new DateTime(2007, 6, 1),
				CreatePV(publishDate: new DateTime(2010, 1, 1))
			);
		}
	}
}
