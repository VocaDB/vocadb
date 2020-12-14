#nullable disable

using System.Collections.Generic;
using VocaDb.Model.DataContracts.Albums;

namespace VocaDb.Web.Models.Shared.Partials.Album
{
	public class AlbumThumbsViewModel
	{
		public AlbumThumbsViewModel(IEnumerable<AlbumForApiContract> albums)
		{
			Albums = albums;
		}

		public IEnumerable<AlbumForApiContract> Albums { get; set; }
	}
}