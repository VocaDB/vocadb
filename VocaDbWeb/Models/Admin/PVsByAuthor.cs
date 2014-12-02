using System.Collections.Generic;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Admin {

	public class PVsByAuthor {

		public PVsByAuthor() { }

		public PVsByAuthor(string author, IEnumerable<PVForSongContract> pvs) {

			ParamIs.NotNull(() => author);
			ParamIs.NotNull(() => pvs);

			Author = author;
			PVs = pvs.ToArray();

		}

		public string Author { get; set; }

		public PVForSongContract[] PVs { get; set; }

	}

}