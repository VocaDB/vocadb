#nullable disable

using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EntryStatusMessageViewModel
	{
		public EntryStatusMessageViewModel(EntryStatus status)
		{
			Status = status;
		}

		public EntryStatus Status { get; set; }
	}
}