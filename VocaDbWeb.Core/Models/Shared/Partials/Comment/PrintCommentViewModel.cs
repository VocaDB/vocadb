#nullable disable

using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.Comment
{
	public class PrintCommentViewModel
	{
		public PrintCommentViewModel(CommentContract contract, bool allowDelete, bool alwaysAllowDelete = false, int maxLength = int.MaxValue)
		{
			Contract = contract;
			AllowDelete = allowDelete;
			AlwaysAllowDelete = alwaysAllowDelete;
			MaxLength = maxLength;
		}

		public CommentContract Contract { get; set; }

		public bool AllowDelete { get; set; }

		public bool AlwaysAllowDelete { get; set; }

		public int MaxLength { get; set; }
	}
}