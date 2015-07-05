using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Ext {

	public class EmbedSongViewModel {

		public int ContainerHeight {
			get {
				return PlayerHeight + 55;
			}
		}

		public PVContract CurrentPV { get; set; }

		public int? Height { get; set; }

		public SongWithPVAndVoteContract Song { get; set; }

		public int? Width { get; set; }

		public PVContract[] MainPVs {
			get {
				return PVHelper.GetMainPVs(Song.PVs);				
			}
		}

		public int PlayerHeight {
			get {
				return Height ?? 315;
			}
		}

		public int PlayerWidth {
			get {
				return Width ?? 560;
			}
		}

	}

}