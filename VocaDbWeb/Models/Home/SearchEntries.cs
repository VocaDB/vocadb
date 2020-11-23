using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.Home
{

	public class SearchEntries
	{

		public SearchEntries() { }

		public SearchEntries(string filter,
			PartialFindResult<AlbumContract> albums,
			PartialFindResult<ArtistContract> artists,
			PartialFindResult<SongWithAlbumContract> songs,
			PartialFindResult<TagContract> tags)
		{

			Filter = filter;
			Albums = albums;
			Artists = artists;
			Songs = songs;
			Tags = tags;

		}

		public PartialFindResult<AlbumContract> Albums { get; set; }

		public PartialFindResult<ArtistContract> Artists { get; set; }

		public string Filter { get; set; }

		public PartialFindResult<SongWithAlbumContract> Songs { get; set; }

		public PartialFindResult<TagContract> Tags { get; set; }

	}

}