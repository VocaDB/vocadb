using VocaDb.Model.Service.Search;

namespace VocaDb.Web.Models.Shared.Partials.Search
{
	public class AdvancedFilterViewModel
	{
		public AdvancedFilterViewModel(string description, AdvancedFilterType filterType, string param, bool negate)
		{
			Description = description;
			FilterType = filterType;
			Param = param;
			Negate = negate;
		}

		public string Description { get; set; }

		public AdvancedFilterType FilterType { get; set; }

		public string Param { get; set; }

		public bool Negate { get; set; }
	}
}