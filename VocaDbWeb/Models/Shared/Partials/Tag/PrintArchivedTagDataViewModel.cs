#nullable disable

using VocaDb.Model.DataContracts.Tags;

namespace VocaDb.Web.Models.Shared.Partials.Tag
{
	public class PrintArchivedTagDataViewModel
	{
		public PrintArchivedTagDataViewModel(ComparedTagsContract comparedTags)
		{
			ComparedTags = comparedTags;
		}

		public ComparedTagsContract ComparedTags { get; set; }
	}
}