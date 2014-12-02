using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VocaDb.Model.Service;

namespace VocaDb.Web.Models.SongLists {

	public class CreateFromWVR {

		public WVRListResult ListResult { get; set; }

		[Display(Name = "Include all songs in the list (not just ranked)")]
		public bool ParseAll { get; set; }

		[Display(Name= "Url")]
		[Required]
		public string Url { get; set; }

	}

}