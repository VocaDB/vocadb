#nullable disable

namespace VocaDb.Web.Models.Shared.Partials.Song
{
	public class SongLinkKnockoutViewModel
	{
		public SongLinkKnockoutViewModel(string binding, string extUrlBinding = null, bool tooltip = false, string toolTipDomainBinding = null)
		{
			Binding = binding;
			ExtUrlBinding = extUrlBinding;
			Tooltip = tooltip;
			ToolTipDomainBinding = toolTipDomainBinding;
		}

		public string Binding { get; set; }

		public string ExtUrlBinding { get; set; }

		public bool Tooltip { get; set; }

		public string ToolTipDomainBinding { get; set; }
	}
}