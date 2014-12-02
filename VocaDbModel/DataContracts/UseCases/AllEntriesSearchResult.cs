using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.UseCases {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AllEntriesSearchResult {

		public AllEntriesSearchResult()
			: this(string.Empty,
			new PartialFindResult<AlbumContract>(), 
			new PartialFindResult<ArtistContract>(),
			new PartialFindResult<SongWithAlbumContract>(),
			new PartialFindResult<TagContract>()) { 
		}

		public AllEntriesSearchResult(string query, 
			PartialFindResult<AlbumContract> albums, 
			PartialFindResult<ArtistContract> artists,
			PartialFindResult<SongWithAlbumContract> songs,
			PartialFindResult<TagContract> tags) {

			Query = query;
			Albums = albums;
			Artists = artists;
			Songs = songs;
			Tags = tags;

		}

		[DataMember]
		public PartialFindResult<AlbumContract> Albums { get; set; }

		[DataMember]
		public PartialFindResult<ArtistContract> Artists { get; set; }

		[DataMember]
		public PartialFindResult<SongWithAlbumContract> Songs { get; set; }

		[DataMember]
		public string Query { get; set; }

		[DataMember]
		public PartialFindResult<TagContract> Tags { get; set; }

		public bool OnlyOneItem {
			get {
				return (Albums.TotalCount + Artists.TotalCount + Songs.TotalCount + Tags.TotalCount == 1);
			}
		}

	}

}
