namespace VocaDb.Web.Models.Shared.Partials.Shared
{
	public class DraftMessageViewModel
	{
		public DraftMessageViewModel(string section)
		{
			Section = section;
		}

		public string Section { get; set; }
	}
}