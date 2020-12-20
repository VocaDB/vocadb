#nullable disable

using System.Collections.Generic;
using VocaDb.Model.DataContracts.Artists;

namespace VocaDb.Web.Models.Shared.Partials.Artist
{
	public class ArtistGridViewModel
	{
		public ArtistGridViewModel(IEnumerable<ArtistContract> artists, int columns, bool displayType = false)
		{
			Artists = artists;
			Columns = columns;
			DisplayType = displayType;
		}

		public IEnumerable<ArtistContract> Artists { get; set; }

		public int Columns { get; set; }

		public bool DisplayType { get; set; }
	}
}