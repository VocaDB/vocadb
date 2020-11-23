using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Service.Search.AlbumSearch
{

	public class AlbumQueryBuilder
	{

		public QueryPlan<Album> BuildPlan(string query)
		{

			var words = SearchParser.ParseQuery(query);
			var filters = new List<ISearchFilter<Album>>();

			var names = words.TakeAll(string.Empty);

			if (names.Any())
			{
				filters.Add(new AlbumNameFilter(names.Select(n => n.Value)));
			}

			var artists = words.TakeAll("artist");
			if (artists.Any())
			{
				filters.Add(new AlbumArtistNameFilter(artists.Select(n => n.Value)));
			}

			while (words.Any())
			{

				var word = words.TakeNext();
				ISearchFilter<Album> filter = null;

				switch (word.PropertyName.ToLowerInvariant())
				{
					case "artistId":
						int artistId;
						if (int.TryParse(word.Value, out artistId))
							filter = new AlbumArtistFilter(artistId);
						break;

					default:
						filter = new AlbumNameFilter(new[] { word.Value });
						break;

				}

				if (filter != null)
					filters.Add(filter);

			}

			return new QueryPlan<Album>(filters);

		}

	}

}
