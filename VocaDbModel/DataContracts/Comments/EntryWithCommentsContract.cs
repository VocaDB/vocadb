#nullable disable

using VocaDb.Model.DataContracts.Api;

namespace VocaDb.Model.DataContracts.Comments
{
	public class EntryWithCommentsContract
	{
		public EntryWithCommentsContract() { }

		public EntryWithCommentsContract(EntryForApiContract entry, CommentForApiContract[] comments)
		{
			Comments = comments;
			Entry = entry;
		}

		public CommentForApiContract[] Comments { get; set; }

		public EntryForApiContract Entry { get; set; }
	}
}
