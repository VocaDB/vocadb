using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Web.Models.Shared.Partials.Tag
{

	public class TagLinkViewModel
	{

		public TagLinkViewModel(TagBaseContract tag, bool tooltip = false)
		{
			Tag = tag;
			Tooltip = tooltip;
		}

		public TagBaseContract Tag { get; set; }

		public bool Tooltip { get; set; }

	}

}