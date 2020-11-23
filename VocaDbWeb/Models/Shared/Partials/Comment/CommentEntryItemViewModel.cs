using VocaDb.Model.DataContracts.Api;

namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class CommentEntryItemViewModel
	{
		public CommentEntryItemViewModel(EntryForApiContract entry)
		{
			Entry = entry;
		}

		public EntryForApiContract Entry { get; set; }
	}
}