#nullable disable

using System.Web;

namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class CommentKnockoutViewModel
	{
		public CommentKnockoutViewModel(string messageBinding, bool allowMarkdown, string deleteHandler = "$parent.deleteComment", string editHandler = null, bool standalone = true, IHtmlString body = null)
		{
			MessageBinding = messageBinding;
			AllowMarkdown = allowMarkdown;
			DeleteHandler = deleteHandler;
			EditHandler = editHandler;
			Standalone = standalone;
			Body = body;
		}

		public string MessageBinding { get; set; }

		public bool AllowMarkdown { get; set; }

		public string DeleteHandler { get; set; }

		public string EditHandler { get; set; }

		public bool Standalone { get; set; }

		public IHtmlString Body { get; set; }
	}
}