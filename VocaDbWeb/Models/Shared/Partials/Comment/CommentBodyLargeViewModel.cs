using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.Comment
{

	public class CommentBodyLargeViewModel
	{

		public CommentBodyLargeViewModel(CommentContract contract, bool allowDelete, bool alwaysAllowDelete = false)
		{
			Contract = contract;
			AllowDelete = allowDelete;
			AlwaysAllowDelete = alwaysAllowDelete;
		}

		public CommentContract Contract { get; set; }

		public bool AllowDelete { get; set; }

		public bool AlwaysAllowDelete { get; set; }

	}

}