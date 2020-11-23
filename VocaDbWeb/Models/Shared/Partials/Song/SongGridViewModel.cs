using System.Collections.Generic;
using VocaDb.Model.DataContracts.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{

	public class SongGridViewModel
	{

		public SongGridViewModel(IEnumerable<SongForApiContract> songs, int columns, bool displayType = false, bool displayPublishDate = false)
		{
			Songs = songs;
			Columns = columns;
			DisplayType = displayType;
			DisplayPublishDate = displayPublishDate;
		}

		public IEnumerable<SongForApiContract> Songs { get; set; }

		public int Columns { get; set; }

		public bool DisplayType { get; set; }

		public bool DisplayPublishDate { get; set; }

	}

}