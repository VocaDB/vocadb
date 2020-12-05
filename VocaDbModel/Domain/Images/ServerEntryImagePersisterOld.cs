using System.Web;
using VocaDb.Model.Domain.Web;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Images
{
	/// <summary>
	/// Stores entry images on disk, using the old directory structure (such as /EntryImg/Album/39-t.jpg).
	/// Used for persisting songlist/tag thumbnails on the server disk.
	/// </summary>
	/// <remarks>
	/// These images are saved in the EntryImg folder under the main application folder for now,
	/// but should be migrated to the static files folder.
	/// </remarks>
	public class ServerEntryImagePersisterOld : ServerEntryImagePersisterBase, IEntryImagePersisterOld, IEntryPictureFilePersister
	{
		public ServerEntryImagePersisterOld(IHttpContext context)
		{
			this.context = context;
		}

		private readonly IHttpContext context;


		private static string GetFileName(int id, string mime, string suffix)
		{
			return string.Format("{0}{1}{2}", id, suffix, ImageHelper.GetExtensionFromMime(mime));
		}

		private static string GetFileName(IEntryImageInformation picture, ImageSize size)
		{
			return GetFileName(picture.Id, picture.Mime, GetSuffix(size));
		}

		private static string GetSuffix(ImageSize size)
		{
			switch (size)
			{
				case ImageSize.Thumb:
					return "-t";
				case ImageSize.SmallThumb:
					return "-st";
				case ImageSize.TinyThumb:
					return "-tt";
				default:
					return string.Empty;
			}
		}

		public override VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
		{
			ParamIs.NotNull(() => picture);

			string url;

			if (picture.Version > 0)
			{
				url = string.Format("/EntryImg/{0}/{1}?v={2}", picture.EntryType, GetFileName(picture, size), picture.Version);
			}
			else
			{
				url = string.Format("/EntryImg/{0}/{1}", picture.EntryType, GetFileName(picture, size));
			}

			return new VocaDbUrl(url, UrlDomain.Main, System.UriKind.Relative);
		}

		public override string GetPath(IEntryImageInformation picture, ImageSize size)
		{
			return context.ServerPathMapper.MapPath(string.Format("~\\EntryImg\\{0}\\{1}", picture.EntryType, GetFileName(picture, size)));
		}

		public override bool IsSupported(IEntryImageInformation picture, ImageSize size)
		{
			return picture.EntryType == EntryType.SongList || picture.EntryType == EntryType.Tag
				|| ((picture.EntryType == EntryType.Album || picture.EntryType == EntryType.Artist) && picture.Purpose == ImagePurpose.Additional);
		}
	}
}
