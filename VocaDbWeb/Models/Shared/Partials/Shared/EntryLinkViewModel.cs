using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class EntryLinkViewModel
	{
		public EntryLinkViewModel(EntryBaseContract entry)
		{
			Entry = entry;
		}

		public EntryBaseContract Entry { get; set; }
	}
}