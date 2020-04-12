using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Web.Code;

namespace VocaDb.Tests.TestSupport {

	public class FakeDynamicImageUrlFactory : IDynamicImageUrlFactory {

		public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size) {
			return new VocaDbUrl($"{picture.EntryType}/{picture.Id}/{(size == ImageSize.Original ? "Picture" : "PictureThumb")}", UrlDomain.Main, System.UriKind.Relative);
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return new DynamicImageUrlFactory(new System.Web.Mvc.UrlHelper()).HasImage(picture, size);
		}

		public bool IsSupported(IEntryImageInformation picture, ImageSize size) {
			return new DynamicImageUrlFactory(new System.Web.Mvc.UrlHelper()).IsSupported(picture, size);
		}
	}

}
