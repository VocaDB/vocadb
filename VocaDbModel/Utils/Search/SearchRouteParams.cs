using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.Utils.Search {

	public class SearchRouteParams {

		public SearchRouteParams(EntryType? searchType = null) {
			this.searchType = searchType;
		}

		public int? artistId { get; set; }

		public ArtistType? artistType { get; set; }

		public DiscType? discType { get; set; }

		public string filter { get; set; }

		public bool? onlyWithPVs { get; set; }

		public EntryType? searchType { get; set; }

		public SongType? songType { get; set; }

		public object sort { get; set; }

		public string tag { get; set; }

	}

	public class SearchRouteParamsFactory {

		public static SearchRouteParamsFactory Instance {
			get { return new SearchRouteParamsFactory(); }
		}

		public SearchRouteParams Albums(string tag = null) {
			
			return new SearchRouteParams(EntryType.Album) {
				tag = tag,
			};

		}

		public SearchRouteParams Artists(string tag = null) {
			
			return new SearchRouteParams(EntryType.Artist) {
				tag = tag,
			};

		}

		public SearchRouteParams Songs(
			int? artistId = null, 
			SongType? songType = null, 
			object sort = null, 
			string tag = null) {
			
			return new SearchRouteParams(EntryType.Song) {
				artistId = artistId, songType =  songType, 
				sort = sort, tag = tag,
			};

		}

	}

}
