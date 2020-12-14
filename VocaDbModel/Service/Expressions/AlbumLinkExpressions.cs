#nullable disable

using System;
using System.Linq.Expressions;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service.Expressions
{
	public static class AlbumLinkExpressions
	{
		public static Expression<Func<T, bool>> WhereAlbumHasVariousArtists<T>() where T : IAlbumLink
		{
			return (T a) => a.Album.ArtistString.Default == ArtistHelper.VariousArtists;
		}

		public static Expression<Func<T, bool>> WhereAlbumDoesNotHaveVariousArtists<T>() where T : IAlbumLink
		{
			return (T a) => a.Album.ArtistString.Default == ArtistHelper.VariousArtists;
		}
	}
}
