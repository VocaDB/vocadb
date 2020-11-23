using VocaDb.Model.Domain;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{

	public class CurrentVersionMessageViewModel
	{

		public CurrentVersionMessageViewModel(int version, EntryStatus status)
		{
			Version = version;
			Status = status;
		}

		public int Version { get; set; }

		public EntryStatus Status { get; set; }

	}

}