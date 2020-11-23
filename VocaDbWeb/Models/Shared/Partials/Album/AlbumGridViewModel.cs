using System.Collections.Generic;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Shared.Partials.Album
{

	public class AlbumGridViewModel
	{

		public AlbumGridViewModel(IEnumerable<AlbumContract> albums, int columns, bool displayRating, bool displayReleaseDate, bool displayType = false)
		{
			Albums = albums;
			Columns = columns;
			DisplayRating = displayRating;
			DisplayReleaseDate = displayReleaseDate;
			DisplayType = displayType;
		}

		public IEnumerable<AlbumContract> Albums { get; set; }

		public int Columns { get; set; }

		public bool DisplayRating { get; set; }

		public bool DisplayReleaseDate { get; set; }

		public bool DisplayType { get; set; }

	}

}