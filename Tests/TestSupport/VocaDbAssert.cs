#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.TestSupport
{
	public static class VocaDbAssert
	{
		public static void ContainsArtists(IEnumerable<IArtistLinkWithRoles> artistLinks, params string[] artistNames)
		{
			foreach (var artistName in artistNames)
			{
				artistLinks.Any(a => a.Artist != null && artistName.Equals(a.Artist.DefaultName)).Should().BeTrue($"Found artist '{artistName}'");
			}
		}

		public static void HasArtist(Album album, string artistName, ArtistRoles? roles)
		{
			var link = album.AllArtists.FirstOrDefault(a => (a.Artist != null && a.Artist.DefaultName.Equals(artistName)) || (a.Artist == null && string.Equals(a.Name, artistName)));

			link.Should().NotBeNull($"Artist '{artistName}' exists for {album}");

			if (roles.HasValue)
				link.Roles.Should().Be(roles, $"Roles for {link}");
		}

		public static void HasArtist(Album album, Artist artist, ArtistRoles? roles)
		{
			var link = album.GetArtistLink(artist);

			link.Should().NotBeNull($"{artist} exists for {album}");

			if (roles.HasValue)
				link.Roles.Should().Be(roles, $"Roles for {link}");
		}

		public static void HasSong(Album album, Song song, int trackNumber)
		{
			var link = album.AllSongs.FirstOrDefault(s => s.Song != null && s.Song.Equals(song));

			link.Should().NotBeNull($"{song} exists for {album}");

			link.TrackNumber.Should().Be(trackNumber, $"Track number for {link}");
		}

		public static void HasSong(Album album, string songName, int trackNumber)
		{
			var link = album.AllSongs.FirstOrDefault(s => string.Equals(s.Name, songName));

			link.Should().NotBeNull($"Song '{songName}' exists for {album}");

			link.TrackNumber.Should().Be(trackNumber, $"Track number for {link}");
		}
	}
}
