using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Models.Shared.Partials.Album
{

	public class CoverLinkViewModel
	{

		public CoverLinkViewModel(IEntryImageInformation imageInfo)
		{
			ImageInfo = imageInfo;
		}

		public IEntryImageInformation ImageInfo { get; set; }

	}

}