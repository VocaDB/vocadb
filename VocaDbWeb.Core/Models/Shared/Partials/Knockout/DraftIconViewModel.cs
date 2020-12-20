#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class DraftIconViewModel
	{
		public DraftIconViewModel(string statusBinding)
		{
			StatusBinding = statusBinding;
		}

		public string StatusBinding { get; set; }
	}
}