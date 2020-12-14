#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class ArtistLockingAutoCompleteViewModel
	{
		public ArtistLockingAutoCompleteViewModel(string binding, string artistTypes, int ignoreId = 0)
		{
			Binding = binding;
			ArtistTypes = artistTypes;
			IgnoreId = ignoreId;
		}

		public string Binding { get; set; }

		public string ArtistTypes { get; set; }

		public int IgnoreId { get; set; }
	}
}