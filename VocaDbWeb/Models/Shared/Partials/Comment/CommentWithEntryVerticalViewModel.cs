using VocaDb.Model.DataContracts.Comments;

namespace VocaDb.Web.Models.Shared.Partials.Comment
{

	public class CommentWithEntryVerticalViewModel
	{

		public CommentWithEntryVerticalViewModel(EntryWithCommentsContract entry, int maxLength = int.MaxValue)
		{
			Entry = entry;
			MaxLength = maxLength;
		}

		public EntryWithCommentsContract Entry { get; set; }

		public int MaxLength { get; set; }

	}

}