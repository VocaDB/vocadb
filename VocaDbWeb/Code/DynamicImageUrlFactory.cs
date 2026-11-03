using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Web.Code;

public class DynamicImageUrlFactory : IDynamicImageUrlFactory
{
	public DynamicImageUrlFactory(LinkGenerator linkGenerator)
	{
		_linkGenerator = linkGenerator;
	}

	private readonly LinkGenerator _linkGenerator;

	public VocaDbUrl GetUrl(IEntryImageInformation imageInfo, ImageSize size)
	{
		string? dynamicUrl = null;
		if (imageInfo.EntryType == EntryType.Album)
		{
			if (size == ImageSize.Original)
				dynamicUrl = _linkGenerator.GetPathByAction("CoverPicture", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
			else
				dynamicUrl = _linkGenerator.GetPathByAction("CoverPictureThumb", "Album", new { id = imageInfo.Id, v = imageInfo.Version });
		}
		else if (imageInfo.EntryType == EntryType.Artist)
		{
			if (size == ImageSize.Original)
				dynamicUrl = _linkGenerator.GetPathByAction("Picture", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
			else
				dynamicUrl = _linkGenerator.GetPathByAction("PictureThumb", "Artist", new { id = imageInfo.Id, v = imageInfo.Version });
		}

		return !string.IsNullOrEmpty(dynamicUrl) ? new VocaDbUrl(dynamicUrl, UrlDomain.Main, System.UriKind.Relative) : VocaDbUrl.Empty;
	}

	public bool HasImage(IEntryImageInformation picture, ImageSize size) => IsSupported(picture, size) && picture.ShouldExist();

	public bool IsSupported(IEntryImageInformation picture, ImageSize size)
	{
		return picture.PurposeMainOrUnspecified() && (picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album);
	}
}