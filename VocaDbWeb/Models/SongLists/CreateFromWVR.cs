using System.ComponentModel.DataAnnotations;
using VocaDb.Model.DataContracts.SongImport;

namespace VocaDb.Web.Models.SongLists {

	public class CreateFromWVR {

		public ImportedSongListContract ListResult { get; set; }

		public bool OnlyRanked { get; set; }

		[Required]
		public string Url { get; set; }

	}

}