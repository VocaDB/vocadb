#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class NameLinkKnockoutViewModel
	{
		public NameLinkKnockoutViewModel(string userBinding)
		{
			UserBinding = userBinding;
		}

		public string UserBinding { get; set; }
	}
}