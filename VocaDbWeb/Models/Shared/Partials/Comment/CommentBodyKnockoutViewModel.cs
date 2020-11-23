namespace VocaDb.Web.Models.Shared.Partials.Comment
{

	public class CommentBodyKnockoutViewModel
	{

		public CommentBodyKnockoutViewModel(string messageBinding)
		{
			MessageBinding = messageBinding;
		}

		public string MessageBinding { get; set; }

	}

}