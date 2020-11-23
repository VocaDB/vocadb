namespace VocaDb.Web.Models.Shared.Partials.Song
{

	public class PVPreviewKnockoutViewModel
	{

		public PVPreviewKnockoutViewModel(string getPvServiceIcons)
		{
			GetPvServiceIcons = getPvServiceIcons;
		}

		public string GetPvServiceIcons { get; set; }

	}

}