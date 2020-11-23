using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class DraftIconViewModel
	{
		public DraftIconViewModel(EntryStatus status)
		{
			Status = status;
		}

		public EntryStatus Status { get; set; }
	}
}