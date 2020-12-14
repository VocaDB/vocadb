#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class IconAndNameLinkKnockoutViewModel
	{
		public IconAndNameLinkKnockoutViewModel(int iconSize = 20, string cssClass = "")
		{
			IconSize = iconSize;
			CssClass = cssClass;
		}

		public int IconSize { get; set; }

		public string CssClass { get; set; }
	}
}