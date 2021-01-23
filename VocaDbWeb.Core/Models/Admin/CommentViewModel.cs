#nullable disable

using System;
using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Admin
{
	[Obsolete]
	public class CommentViewModel
	{
		public CommentViewModel(CommentForApiContract comment, string targetName, string targetUrl)
		{
			Comment = comment;
			TargetName = targetName;
			TargetUrl = targetUrl;
		}

		public CommentForApiContract Comment { get; set; }

		public string TargetName { get; set; }

		public string TargetUrl { get; set; }
	}
}