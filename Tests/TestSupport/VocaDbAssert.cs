#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
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
				Assert.IsTrue(artistLinks.Any(a => a.Artist != null && artistName.Equals(a.Artist.DefaultName)),
					string.Format("Found artist '{0}'", artistName));
			}
		}

		public static void HasArtist(Album album, string artistName, ArtistRoles? roles)
		{
			var link = album.AllArtists.FirstOrDefault(a => (a.Artist != null && a.Artist.DefaultName.Equals(artistName)) || (a.Artist == null && string.Equals(a.Name, artistName)));

			Assert.IsNotNull(link, string.Format("Artist '{0}' exists for {1}", artistName, album));

			if (roles.HasValue)
				Assert.AreEqual(roles, link.Roles, string.Format("Roles for {0}", link));
		}

		public static void HasArtist(Album album, Artist artist, ArtistRoles? roles)
		{
			var link = album.GetArtistLink(artist);

			Assert.IsNotNull(link, string.Format("{0} exists for {1}", artist, album));

			if (roles.HasValue)
				Assert.AreEqual(roles, link.Roles, string.Format("Roles for {0}", link));
		}

		public static void HasSong(Album album, Song song, int trackNumber)
		{
			var link = album.AllSongs.FirstOrDefault(s => s.Song != null && s.Song.Equals(song));

			Assert.IsNotNull(link, string.Format("{0} exists for {1}", song, album));

			Assert.AreEqual(trackNumber, link.TrackNumber, string.Format("Track number for {0}", link));
		}

		public static void HasSong(Album album, string songName, int trackNumber)
		{
			var link = album.AllSongs.FirstOrDefault(s => string.Equals(s.Name, songName));

			Assert.IsNotNull(link, string.Format("Song '{0}' exists for {1}", songName, album));

			Assert.AreEqual(trackNumber, link.TrackNumber, string.Format("Track number for {0}", link));
		}
	}
}
