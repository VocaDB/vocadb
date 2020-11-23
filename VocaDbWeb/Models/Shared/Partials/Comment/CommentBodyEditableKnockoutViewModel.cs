namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class CommentBodyEditableKnockoutViewModel
	{
		public CommentBodyEditableKnockoutViewModel(string messageBinding)
		{
			MessageBinding = messageBinding;
		}

		public string MessageBinding { get; set; }
	}
}