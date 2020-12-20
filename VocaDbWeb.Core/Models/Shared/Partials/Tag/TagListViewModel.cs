#nullable disable

using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Web.Models.Shared.Partials.Tag
{
	public class TagListViewModel
	{
		public TagListViewModel(TagBaseContract[] tagNames, bool tooltip = false)
		{
			TagNames = tagNames;
			Tooltip = tooltip;
		}

		public TagBaseContract[] TagNames { get; set; }

		public bool Tooltip { get; set; }
	}
}