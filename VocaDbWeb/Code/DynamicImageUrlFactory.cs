using System.Web.Mvc;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Utils;

namespace VocaDb.Web.Code {

	public class DynamicImageUrlFactory : IDynamicImageUrlFactory {

		public DynamicImageUrlFactory(UrlHelper urlHelper) {
			this.urlHelper = urlHelper;
		}

		private readonly UrlHelper urlHelper;

		public string GetUrlAbsolute(IEntryImageInformation imageInfo, ImageSize size) {
			
			string dynamicUrl = null;
			if (imageInfo.EntryType == EntryType.Album) {
				if (size == ImageSize.Original)
					dynamicUrl = urlHelper.Action("CoverPicture", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
				else
					dynamicUrl = urlHelper.Action("CoverPictureThumb", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
			} else if (imageInfo.EntryType == EntryType.Artist) {				
				if (size == ImageSize.Original)
					dynamicUrl = urlHelper.Action("Picture", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
				else
					dynamicUrl = urlHelper.Action("PictureThumb", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
			}

			return !string.IsNullOrEmpty(dynamicUrl) ? VocaUriBuilder.Absolute(dynamicUrl) : null;

		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return (picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album) 
				&& picture.PurposeMainOrUnspecified() 
				&& picture.ShouldExist();
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) {
			return picture.PurposeMainOrUnspecified() && (picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album);
		}
	}

}