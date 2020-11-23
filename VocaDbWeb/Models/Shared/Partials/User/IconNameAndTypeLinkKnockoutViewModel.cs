namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class IconNameAndTypeLinkKnockoutViewModel
	{
		public IconNameAndTypeLinkKnockoutViewModel(string groupResources, int iconSize = 20)
		{
			GroupResources = groupResources;
			IconSize = iconSize;
		}

		public string GroupResources { get; set; }

		public int IconSize { get; set; }
	}
}