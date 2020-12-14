#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Knockout
{
	public class TagFiltersViewModel
	{
		public TagFiltersViewModel(bool topGenres = false)
		{
			TopGenres = topGenres;
		}

		public bool TopGenres { get; set; }
	}
}