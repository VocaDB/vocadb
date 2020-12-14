#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class CreateCommentViewModel
	{
		public CreateCommentViewModel(string cssClass, int newCommentRows)
		{
			CssClass = cssClass;
			NewCommentRows = newCommentRows;
		}

		public string CssClass { get; set; }

		public int NewCommentRows { get; set; }
	}
}