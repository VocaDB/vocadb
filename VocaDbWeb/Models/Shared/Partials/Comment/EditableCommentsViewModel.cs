#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class EditableCommentsViewModel
	{
		public EditableCommentsViewModel(bool allowCreateComment, bool well = true, string commentsBinding = "pageOfComments", int newCommentRows = 6, bool commentBoxEnd = false, bool pagination = true)
		{
			AllowCreateComment = allowCreateComment;
			Well = well;
			CommentsBinding = commentsBinding;
			NewCommentRows = newCommentRows;
			CommentBoxEnd = commentBoxEnd;
			Pagination = pagination;
		}

		public bool AllowCreateComment { get; set; }

		public bool Well { get; set; }

		public string CommentsBinding { get; set; }

		public int NewCommentRows { get; set; }

		public bool CommentBoxEnd { get; set; }

		public bool Pagination { get; set; }
	}
}