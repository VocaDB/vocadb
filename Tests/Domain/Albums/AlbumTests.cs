#nullable disable

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Domain.Albums
{
	[TestClass]
	public class AlbumTests
	{
		private Album _album;
		private Artist _producer;
		private ArtistContract _producerContract;
		private Artist _vocalist;
		private ArtistContract _vocalistContract;
		private Song _song1;
		private SongInAlbumEditContract _songInAlbumContract;

		private void AssertEquals(SongInAlbum first, SongInAlbumEditContract second, string message)
		{
			Album.TrackPropertiesEqual(first, second).Should().BeTrue(message);
			Album.TrackArtistsEqual(first.Song, second).Should().BeTrue(message);
		}

		private SongInAlbumEditContract CreateSongInAlbumEditContract(int id, string name, int trackNum)
		{
			return new SongInAlbumEditContract { SongInAlbumId = id, IsCustomTrack = true, SongName = name, TrackNumber = trackNum, DiscNumber = 1, Artists = new ArtistContract[0] };
		}

		private Task<List<Artist>> GetArtists(ArtistContract[] contracts)
		{
			return Task.FromResult(contracts
				.Select(a => a.Id == _vocalist.Id ? _vocalist : new Artist(TranslatedString.Create(a.Name)) { Id = a.Id })
				.ToList());
		}

		private Task<Song> GetSong(SongInAlbumEditContract contract)
		{
			if (contract.SongId == _song1.Id)
				return Task.FromResult(_song1);

			return Task.FromResult(new Song(new LocalizedString(contract.SongName, ContentLanguageSelection.Unspecified)));
		}

		private Task UpdateSongArtists(Song song, ArtistContract[] artists)
		{
			return song.SyncArtistsAsync(artists, GetArtists);
		}

		private Task<CollectionDiffWithValue<SongInAlbum, SongInAlbum>> SyncSongs(SongInAlbumEditContract[] newSongs)
		{
			return _album.SyncSongs(newSongs, GetSong, UpdateSongArtists);
		}

		[TestInitialize]
		public void SetUp()
		{
			_album = new Album(new LocalizedString("Synthesis", ContentLanguageSelection.English));

			_producer = new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer };
			_vocalist = new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 39, ArtistType = ArtistType.Vocaloid };
			_producerContract = new ArtistContract(_producer, ContentLanguagePreference.Default);
			_vocalistContract = new ArtistContract(_vocalist, ContentLanguagePreference.Default);

			_song1 = new Song(new LocalizedString("Nebula", ContentLanguageSelection.English)) { Id = 2 };
			_song1.AddArtist(_producer);

			_album.AddArtist(_producer);
			_album.AddArtist(_vocalist);

			var songInAlbum = new SongInAlbum(_song1, _album, 1, 1);
			_songInAlbumContract = new SongInAlbumEditContract(songInAlbum, ContentLanguagePreference.Default);
			_songInAlbumContract.Artists = new[] { _producerContract };
		}

		[TestMethod]
		public void Ctor_LocalizedString()
		{
			var result = new Album(new LocalizedString("album", ContentLanguageSelection.Romaji));

			result.Names.Count().Should().Be(1, "Names count");
			result.Names.HasNameForLanguage(ContentLanguageSelection.Romaji).Should().BeTrue("Has name for Romaji");
			result.Names.HasNameForLanguage(ContentLanguageSelection.English).Should().BeFalse("Does not have name for English");
			result.Names.GetEntryName(ContentLanguagePreference.Romaji).DisplayName.Should().Be("album", "Display name");
		}

		[TestMethod]
		public void CreateWebLink()
		{
			_album.CreateWebLink("test link", "http://www.test.com", WebLinkCategory.Other, disabled: false);

			_album.WebLinks.Count.Should().Be(1, "Should have one link");
			var link = _album.WebLinks.First();
			link.Description.Should().Be("test link", "description");
			link.Url.Should().Be("http://www.test.com", "url");
			link.Category.Should().Be(WebLinkCategory.Other, "category");
		}

		[TestMethod]
		public async Task SyncSongs_NoExistingLinks()
		{
			var newSongs = new[] { _songInAlbumContract };

			var result = await SyncSongs(newSongs);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(1, "1 added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			AssertEquals(result.Added.First(), _songInAlbumContract, "added song matches contract");
			1.Should().Be(result.Added.First().Song.ArtistList.Count(), "one artist");
		}

		[TestMethod]
		public async Task SyncSongs_NotChanged()
		{
			_album.AddSong(_song1, 1, 1);
			var newSongs = new[] { _songInAlbumContract };

			var result = await SyncSongs(newSongs);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeFalse("is not changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			AssertEquals(result.Unchanged.First(), _songInAlbumContract, "unchanged song matches contract");
		}

		/// <summary>
		/// Edited track properties other than artists.
		/// </summary>
		[TestMethod]
		public async Task SyncSongs_EditedProperties()
		{
			_album.AddSong(_song1, 1, 1);
			_songInAlbumContract.TrackNumber = 2;
			var newSongs = new[] { _songInAlbumContract };

			var result = await SyncSongs(newSongs);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(1, "1 edited");
			result.Removed.Length.Should().Be(0, "none removed");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			AssertEquals(result.Edited.First(), _songInAlbumContract, "edited song matches contract");
			result.Edited.First().TrackNumber.Should().Be(2, "edited song track number is updated");
		}

		/// <summary>
		/// Edited track artists list.
		/// </summary>
		[TestMethod]
		public async Task SyncSongs_EditedArtists()
		{
			_album.AddSong(_song1, 1, 1);
			_songInAlbumContract.Artists = new[] { _producerContract, _vocalistContract };
			var newSongs = new[] { _songInAlbumContract };

			var result = await SyncSongs(newSongs);

			result.Should().NotBeNull("result is not null");
			result.Unchanged.Length.Should().Be(1, "1 unchanged");
			AssertEquals(result.Unchanged.First(), _songInAlbumContract, "edited song matches contract");
			result.Unchanged.First().Song.ArtistString.Default.Should().Be("Tripshots feat. Hatsune Miku", "edited song artist string is updated");
		}

		[TestMethod]
		public async Task SyncSongs_Removed()
		{
			_album.AddSong(_song1, 1, 1);
			var newSongs = new SongInAlbumEditContract[0];

			var result = await SyncSongs(newSongs);

			result.Should().NotBeNull("result is not null");
			result.Changed.Should().BeTrue("is changed");
			result.Added.Length.Should().Be(0, "none added");
			result.Edited.Length.Should().Be(0, "none edited");
			result.Removed.Length.Should().Be(1, "1 removed");
			result.Unchanged.Length.Should().Be(0, "none unchanged");
			AssertEquals(result.Removed.First(), _songInAlbumContract, "removed song matches contract");
		}

		[TestMethod]
		public async Task SyncSongs_AddCustom()
		{
			var link = _album.AddSong("Track 1", 1, 1);
			link.Id = 1;

			var newSongs = new[] {
				CreateSongInAlbumEditContract(1, "Track 1", 1),
				CreateSongInAlbumEditContract(0, "Track 2", 2),
			};

			var result = await SyncSongs(newSongs);

			result.Added.Length.Should().Be(1, "Added");
			result.Unchanged.Length.Should().Be(1, "Edited");
			result.Unchanged.First().Name.Should().Be("Track 1", "Unchanged track name");
			result.Added.First().Name.Should().Be("Track 2", "Added track name");
		}

		/*[TestMethod]
		public void SyncArtists_Duplicate() {

			var newArtists = new[] {
				new ArtistForAlbumContract(new ArtistForAlbum(album, artist, false, ArtistRoles.Default), ContentLanguagePreference.Default),
				new ArtistForAlbumContract(new ArtistForAlbum(album, artist, false, ArtistRoles.Composer), ContentLanguagePreference.Default),
			};

			album.SyncArtists(newArtists, c => c.Artist.Id == artist.Id ? artist : null);

			song.AllArtists.Count.Should().Be(1, "Only one artist");
			song.AllArtists.First().Artist.Should().Be(artist, "Artist is as expected");
		}*/
	}
}
