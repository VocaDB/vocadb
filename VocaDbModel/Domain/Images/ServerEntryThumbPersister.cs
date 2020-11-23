using System;
using System.IO;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Images
{

	/// <summary>
	/// Stores entry thumbnails (for album/artist) in the static files folder on the server disk using the newer directory layout
	/// where different sizes of images are saved in separate folders (for example /img/Album/mainSmall/39.jpg).
	/// </summary>
	public class ServerEntryThumbPersister : ServerEntryImagePersisterBase, IEntryThumbPersister
	{

		private readonly string staticRoot;

		private static string GetDir(ImageSize size)
		{

			switch (size)
			{
				case ImageSize.Original:
					return "Orig";
				case ImageSize.Thumb:
					return "Thumb";
				case ImageSize.SmallThumb:
					return "Small";
				case ImageSize.TinyThumb:
					return "Tiny";
				default:
					throw new NotSupportedException();
			}

		}

		private string GetRelativeUrl(IEntryImageInformation picture, ImageSize size)
		{
			if (picture.Version > 0)
			{
				return string.Format("/img/{0}/main{1}/{2}{3}?v={4}", picture.EntryType.ToString().ToLowerInvariant(), GetDir(size), picture.Id,
					ImageHelper.GetExtensionFromMime(picture.Mime), picture.Version);
			}
			else
				return string.Format("/img/{0}/main{1}/{2}{3}", picture.EntryType.ToString().ToLowerInvariant(), GetDir(size), picture.Id, ImageHelper.GetExtensionFromMime(picture.Mime));
		}

		public override string GetPath(IEntryImageInformation picture, ImageSize size)
		{
			if (string.IsNullOrEmpty(staticRoot))
				return string.Empty;
			var relative = string.Format(@"img\{0}\main{1}\{2}{3}", picture.EntryType, GetDir(size), picture.Id, ImageHelper.GetExtensionFromMime(picture.Mime));
			return Path.Combine(staticRoot, relative);
		}

		public ServerEntryThumbPersister()
		{

			staticRoot = AppConfig.StaticContentPath;

		}

		public override VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
		{
			return new VocaDbUrl(GetRelativeUrl(picture, size), UrlDomain.Static, UriKind.Relative);
		}

		public override bool IsSupported(IEntryImageInformation picture, ImageSize size)
		{
			return picture.EntryType == EntryType.ReleaseEvent || picture.EntryType == EntryType.ReleaseEventSeries
				|| ((picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album)
					&& picture.PurposeMainOrUnspecified()
					&& size != ImageSize.Original);
		}
	}

}
