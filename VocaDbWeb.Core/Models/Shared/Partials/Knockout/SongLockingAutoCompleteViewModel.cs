#nullable disable

using VocaDb.Model.Domain.Songs;

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class SongLockingAutoCompleteViewModel
	{
		public SongLockingAutoCompleteViewModel(string binding, SongTypes songTypes = SongTypes.Unspecified, int ignoreId = 0)
		{
			Binding = binding;
			SongTypes = songTypes;
			IgnoreId = ignoreId;
		}

		public string Binding { get; set; }

		public SongTypes SongTypes { get; set; }

		public int IgnoreId { get; set; }
	}
}