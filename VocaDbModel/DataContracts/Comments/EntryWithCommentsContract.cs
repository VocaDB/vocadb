using VocaDb.Model.DataContracts.Api;

namespace VocaDb.Model.DataContracts.Comments
{
	public class EntryWithCommentsContract
	{
		public EntryWithCommentsContract() { }

		public EntryWithCommentsContract(EntryForApiContract entry, CommentContract[] comments)
		{
			Comments = comments;
			Entry = entry;
		}

		public CommentContract[] Comments { get; set; }

		public EntryForApiContract Entry { get; set; }
	}
}
