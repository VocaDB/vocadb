using System.ComponentModel.DataAnnotations;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.SongLists {

	public class CreateFromWVR {

		public WVRListResult ListResult { get; set; }

		public bool OnlyRanked { get; set; }

		[Required]
		public string Url { get; set; }

	}

}