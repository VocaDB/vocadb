using System;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.Search.SongSearch {

	public class ParsedSongQuery {

		public ParsedSongQuery() {
			ArtistType = ArtistType.Unknown;
		}

		public string ArtistTag { get; set; }

		public ArtistType ArtistType { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public string NicoId { get; set; }

		public DateTime? PublishedAfter { get; set; }

		public DateTime? PublishedBefore { get; set; }

		public string TagName { get; set; }

		public bool HasNameQuery {
			get {
				return !string.IsNullOrEmpty(Name);
			}
		}

	}

}
