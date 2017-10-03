using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Queries {

	/// <summary>
	/// Finds the previous and next main albums for release date for a particular album.
	/// </summary>
	public class PreviousAndNextAlbumsQuery {

		private readonly IDatabaseContext<Album> ctx;

		private IQueryable<Album> AddReleaseRestrictionAfter(IQueryable<Album> criteria, DateTime date) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year > date.Year
				|| (a.OriginalRelease.ReleaseDate.Year == date.Year && a.OriginalRelease.ReleaseDate.Month > date.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == date.Year
					&& a.OriginalRelease.ReleaseDate.Month == date.Month
					&& a.OriginalRelease.ReleaseDate.Day > date.Day)));

		}

		private IQueryable<Album> AddReleaseRestrictionBefore(IQueryable<Album> criteria, DateTime date) {

			return criteria.Where(a => a.OriginalRelease.ReleaseDate.Year != null
				&& a.OriginalRelease.ReleaseDate.Month != null
				&& a.OriginalRelease.ReleaseDate.Day != null
				&& (a.OriginalRelease.ReleaseDate.Year < date.Year
				|| (a.OriginalRelease.ReleaseDate.Year == date.Year && a.OriginalRelease.ReleaseDate.Month < date.Month)
				|| (a.OriginalRelease.ReleaseDate.Year == date.Year
					&& a.OriginalRelease.ReleaseDate.Month == date.Month
					&& a.OriginalRelease.ReleaseDate.Day <= date.Day)));

		}

		private IQueryable<Album> GetMainAlbumQuery(Artist artist, Album album) {

			var albumId = album.Id;
			var producerRoles = ArtistRoles.Composer | ArtistRoles.Arranger;

			return ctx.OfType<ArtistForAlbum>().Query()
				.Where(a => a.Artist.Id == artist.Id && !a.Album.Deleted && a.Album.Id != albumId && !a.IsSupport
				&& ((a.Roles == ArtistRoles.Default) || ((a.Roles & producerRoles) != ArtistRoles.Default))
				&& a.Album.ArtistString.Default != ArtistHelper.VariousArtists)
				.Select(a => a.Album);

		} 

		private Artist[] GetMainArtists(Album album, IEnumerable<IArtistLinkWithRoles> creditableArtists) {

			if (album.ArtistString.Default == ArtistHelper.VariousArtists)
				return null;

			return ArtistHelper.GetProducers(creditableArtists, AlbumHelper.GetContentFocus(album.DiscType)).Select(a => a.Artist).ToArray();

		}

		public PreviousAndNextAlbumsQuery(IDatabaseContext<Album> ctx) {
			this.ctx = ctx;
		}

		public Album[] GetRelatedAlbums(Album album) {

			var creditableArtists = album.Artists.Where(a => a.Artist != null && !a.IsSupport).ToArray();
			Artist mainArtist = null;
			var mainArtists = GetMainArtists(album, creditableArtists);

			if (mainArtists != null && mainArtists.Length == 1)
				mainArtist = mainArtists.First();

			if (mainArtist == null) {

				mainArtists = creditableArtists.Where(a => ArtistHelper.GetCategories(a).HasFlag(ArtistCategories.Circle)).Select(c => c.Artist).ToArray();

				if (mainArtists.Length == 1)
					mainArtist = mainArtists.First();

			}

			if (mainArtist == null)
				return new Album[0];

			if (album.OriginalReleaseDate.IsEmpty) {

				var mainAlbumsQuery = GetMainAlbumQuery(mainArtist, album)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
					.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
					.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
					.Take(2);

				return mainAlbumsQuery.ToArray();

			} else {

				var releaseDate = album.OriginalReleaseDate.ToDateTime();

				var previousAlbum = AddReleaseRestrictionBefore(GetMainAlbumQuery(mainArtist, album), releaseDate)
					.OrderByDescending(a => a.OriginalRelease.ReleaseDate.Year)
					.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Month)
					.ThenByDescending(a => a.OriginalRelease.ReleaseDate.Day)
					.FirstOrDefault();

				var nextAlbum = AddReleaseRestrictionAfter(GetMainAlbumQuery(mainArtist, album), releaseDate)
					.OrderBy(a => a.OriginalRelease.ReleaseDate.Year)
					.ThenBy(a => a.OriginalRelease.ReleaseDate.Month)
					.ThenBy(a => a.OriginalRelease.ReleaseDate.Day)
					.FirstOrDefault();
				
				return new[] { previousAlbum, nextAlbum }.Where(a => a != null).ToArray();

			}

		}

	}

}
