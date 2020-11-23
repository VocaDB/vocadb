using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared.Partials.Html
{

	public class OpenGraphMetaTagsViewModel
	{

		public OpenGraphMetaTagsViewModel(PagePropertiesData pageProperties)
		{
			PageProperties = pageProperties;
		}

		public PagePropertiesData PageProperties { get; set; }

	}

}