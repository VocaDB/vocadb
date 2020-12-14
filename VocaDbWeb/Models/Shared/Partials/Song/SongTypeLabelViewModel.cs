#nullable disable

using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class SongTypeLabelViewModel
	{
		public SongTypeLabelViewModel(SongType songType)
		{
			SongType = songType;
		}

		public SongType SongType { get; set; }
	}
}