#nullable disable

using VocaDb.Model.Domain.Web;
using VocaDb.Model.Helpers;

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
			_context = context;
		}

		private readonly IHttpContext _context;

		private static string GetFileName(int id, string mime, string suffix) => $"{id}{suffix}{ImageHelper.GetExtensionFromMime(mime)}";

		private static string GetFileName(IEntryImageInformation picture, ImageSize size) => GetFileName(picture.Id, picture.Mime, GetSuffix(size));

		private static string GetSuffix(ImageSize size) => size switch
		{
			ImageSize.Thumb => "-t",
			ImageSize.SmallThumb => "-st",
			ImageSize.TinyThumb => "-tt",
			_ => string.Empty,
		};

		public override VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
		{
			ParamIs.NotNull(() => picture);

			var url = (picture.Version > 0)
				? $"/EntryImg/{picture.EntryType}/{GetFileName(picture, size)}?v={picture.Version}"
				: $"/EntryImg/{picture.EntryType}/{GetFileName(picture, size)}";

			return new VocaDbUrl(url, UrlDomain.Main, System.UriKind.Relative);
		}

		public override string GetPath(IEntryImageInformation picture, ImageSize size) => _context.ServerPathMapper.MapPath($@"~\EntryImg\{picture.EntryType}\{GetFileName(picture, size)}");

		public override bool IsSupported(IEntryImageInformation picture, ImageSize size) => (picture.EntryType == EntryType.Album || picture.EntryType == EntryType.Artist) && picture.Purpose == ImagePurpose.Additional;
	}
}
