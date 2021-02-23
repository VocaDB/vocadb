#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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

			_song.PublishDate.DateTime.Should().Be(expected, "PublishDate");
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

			_song.Names.Count().Should().Be(1, "Names count");
			_song.Names.HasNameForLanguage(ContentLanguageSelection.Romaji).Should().BeTrue("Has name for Romaji");
			_song.Names.HasNameForLanguage(ContentLanguageSelection.English).Should().BeFalse("Does not have name for English");
			_song.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName.Should().Be("song", "Display name");
		}

		[TestMethod]
		public void AddTag_NewTag()
		{
			var tag = CreateEntry.Tag("rock");
			var result = _song.AddTag(tag);

			result.Should().NotBeNull("result");
			result.IsNew.Should().BeTrue("Is new");
			result.Result.Tag.Should().Be(tag, "Added tag");
			_song.Tags.Usages.Count.Should().Be(1, "Number of tag usages for song");
			tag.UsageCount.Should().Be(1, "Number of usages for tag");
		}

		[TestMethod]
		public void AddTag_ExistingTag()
		{
			var tag = CreateEntry.Tag("rock");
			_song.AddTag(tag);
			var result = _song.AddTag(tag);

			result.Should().NotBeNull("result");
			result.IsNew.Should().BeFalse("Is new");
			result.Result.Tag.Should().Be(tag, "Added tag");
			_song.Tags.Usages.Count.Should().Be(1, "Number of tag usages for song");
			tag.UsageCount.Should().Be(1, "Number of usages for tag");
		}

		[TestMethod]
		public void LyricsFromParents_NoLyrics()
		{
			var result = CallGetLyricsFromParents(new Song());

			result.Count.Should().Be(0, "no lyrics");
		}

		[TestMethod]
		public void LyricsFromParents_NoParent()
		{
			var result = CallGetLyricsFromParents(_song);

			result.Count.Should().Be(1, "one entry");
			result.First().Should().BeSameAs(_lyrics, "returned lyrics from entry");
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
			result.Count.Should().Be(0, "No lyrics inherited for instrumental");
		}

		[TestMethod]
		public void LyricsFromParents_FromParent()
		{
			var derived = new Song
			{
				OriginalVersion = _song
			};
			var result = CallGetLyricsFromParents(derived);

			result.Count.Should().Be(1, "one entry");
			result.First().Should().BeSameAs(_lyrics, "returned lyrics from entry");
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
			result.FirstOrDefault().Should().BeSameAs(_lyrics, "returned lyrics from entry");
		}

		[TestMethod]
		public void SyncArtists_Duplicate()
		{
			var newArtists = new[] {
				new ArtistForSongContract(new ArtistForSong(_song, _artist, false, ArtistRoles.Default), ContentLanguagePreference.Default),
				new ArtistForSongContract(new ArtistForSong(_song, _artist, false, ArtistRoles.Composer), ContentLanguagePreference.Default),
			};

			_song.SyncArtists(newArtists, c => c.Artist.Id == _artist.Id ? _artist : null);

			_song.AllArtists.Count.Should().Be(1, "Only one artist");
			_song.AllArtists.First().Artist.Should().Be(_artist, "Artist is as expected");
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

			_song.AllArtists.Count.Should().Be(2, "artists count");
			_song.HasArtistLink(link).Should().BeTrue("Still has the extra artist");
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

			_song.Artists.Count().Should().Be(2, "Number of artists");
			var producerLink = _song.Artists.FirstOrDefault(a => a.Artist?.Id == _artist.Id);
			producerLink.Should().NotBeNull("Artist link was added");
			producerLink.Name.Should().Be("RyuuseiP", "Added link name");
			_song.ArtistString.Default.Should().Be("RyuuseiP feat. Hatsune Miku", "Artist string");
		}

		[TestMethod]
		public void UpdatePVServices_None()
		{
			_song.UpdatePVServices();

			_song.PVServices.Should().Be(PVServices.Nothing);
		}

		[TestMethod]
		public void UpdatePVServices_One()
		{
			CreatePV(PVService.Youtube);

			_song.UpdatePVServices();

			_song.PVServices.Should().Be(PVServices.Youtube);
		}

		[TestMethod]
		public void UpdatePVServices_Multiple()
		{
			CreatePV(PVService.NicoNicoDouga);
			CreatePV(PVService.SoundCloud);
			CreatePV(PVService.Youtube);

			_song.UpdatePVServices();

			_song.PVServices.Should().Be(PVServices.NicoNicoDouga | PVServices.SoundCloud | PVServices.Youtube);
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
