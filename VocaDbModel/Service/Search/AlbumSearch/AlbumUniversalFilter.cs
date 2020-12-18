#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.Search.AlbumSearch
{
	public class AlbumUniversalFilter : ISearchFilter<Album>
	{
		private readonly string term;

		public AlbumUniversalFilter(string term)
		{
			this.term = term;
		}

		public QueryCost Cost => QueryCost.VeryHigh;

		public void FilterResults(List<Album> albums, IDatabaseContext session)
		{
			albums.RemoveAll(a => !(
				a.Names.Any(n => n.Value.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1)
				|| (a.ArtistString.Default.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Japanese.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.Romaji.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1
				&& a.ArtistString.English.IndexOf(term, StringComparison.InvariantCultureIgnoreCase) != -1)));
		}

		public List<Album> GetResults(IDatabaseContext session)
		{
			var nameRes = session.Query<AlbumName>().Where(n => n.Value.Contains(term))
				.Select(n => n.Album)
				.Distinct()
				.ToList();

			var artistRes = session.Query<ArtistName>()
				.Where(an => an.Value.Contains(term))
				.SelectMany(an => an.Artist.AllAlbums)
				.Select(an => an.Album)
				.Distinct()
				.ToList();

			var albumRes = session.Query<Album>()
				.Where(an => an.OriginalRelease.CatNum.Contains(term))
				.ToList();

			return nameRes.Union(artistRes).Union(albumRes)
				.ToList();

			/*return session.Query<AlbumName>()
				.Where(n => n.Value.Contains(term))
				.Select(a => a.Album)
				.Where(a => 
					a.ArtistString.Default.Contains(term)
					&& a.ArtistString.Japanese.Contains(term)
					&& a.ArtistString.Romaji.Contains(term)
					&& a.ArtistString.English.Contains(term))
				.ToList();*/
		}

		public IQueryable<Album> Filter(IQueryable<Album> query, IDatabaseContext session)
		{
			throw new NotImplementedException();
		}

		public IQueryable<Album> Query(IDatabaseContext session)
		{
			throw new NotImplementedException();
		}
	}
}
