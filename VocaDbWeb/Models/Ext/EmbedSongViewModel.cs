using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Ext {

	public class EmbedSongViewModel {

		public PVContract CurrentPV { get; set; }

		public int Height { get; set; }

		public SongWithPVAndVoteContract Song { get; set; }

		public int Width { get; set; }

		public PVContract[] GetMainPVs() {
			return PVHelper.GetMainPVs(Song.PVs);
		}

	}

}