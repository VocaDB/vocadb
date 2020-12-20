#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class IconAndLinkKnockoutViewModel
	{
		public IconAndLinkKnockoutViewModel(string userBinding, string cssClass)
		{
			UserBinding = userBinding;
			CssClass = cssClass;
		}

		public string UserBinding { get; set; }

		public string CssClass { get; set; }
	}
}