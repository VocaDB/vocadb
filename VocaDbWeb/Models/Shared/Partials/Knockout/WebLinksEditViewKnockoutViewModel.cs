namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class WebLinksEditViewKnockoutViewModel
	{
		public WebLinksEditViewKnockoutViewModel(bool showCategory = true)
		{
			ShowCategory = showCategory;
		}

		public bool ShowCategory { get; set; }
	}
}