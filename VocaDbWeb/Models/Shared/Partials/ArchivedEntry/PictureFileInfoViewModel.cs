using VocaDb.Model.DataContracts;

namespace VocaDb.Web.Models.Shared.Partials.ArchivedEntry
{
	public class PictureFileInfoViewModel
	{
		public PictureFileInfoViewModel(ArchivedEntryPictureFileContract pic)
		{
			Pic = pic;
		}

		public ArchivedEntryPictureFileContract Pic { get; set; }
	}
}