#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Web.Code;

namespace VocaDb.Tests.TestSupport;

public class FakeDynamicImageUrlFactory : IDynamicImageUrlFactory
{
	public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
	{
		return new VocaDbUrl($"{picture.EntryType}/{picture.Id}/{(size == ImageSize.Original ? "Picture" : "PictureThumb")}", UrlDomain.Main, UriKind.Relative);
	}

	public bool HasImage(IEntryImageInformation picture, ImageSize size)
	{
		return new DynamicImageUrlFactory(linkGenerator: null).HasImage(picture, size);
	}

	public bool IsSupported(IEntryImageInformation picture, ImageSize size)
	{
		return new DynamicImageUrlFactory(linkGenerator: null).IsSupported(picture, size);
	}
}
