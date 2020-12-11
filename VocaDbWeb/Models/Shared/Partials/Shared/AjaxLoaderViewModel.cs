namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class AjaxLoaderViewModel
	{
		public AjaxLoaderViewModel(string id = null)
		{
			Id = id;
		}

		public string Id { get; set; }
	}
}