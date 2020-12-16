#nullable disable

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
		private readonly string _staticRoot;

		private static string GetDir(ImageSize size) => size switch
		{
			ImageSize.Original => "Orig",
			ImageSize.Thumb => "Thumb",
			ImageSize.SmallThumb => "Small",
			ImageSize.TinyThumb => "Tiny",
			_ => throw new NotSupportedException(),
		};

		private string GetRelativeUrl(IEntryImageInformation picture, ImageSize size) => (picture.Version > 0)
			? $"/img/{picture.EntryType.ToString().ToLowerInvariant()}/main{GetDir(size)}/{picture.Id}{ImageHelper.GetExtensionFromMime(picture.Mime)}?v={picture.Version}"
			: $"/img/{picture.EntryType.ToString().ToLowerInvariant()}/main{GetDir(size)}/{picture.Id}{ImageHelper.GetExtensionFromMime(picture.Mime)}";

		public override string GetPath(IEntryImageInformation picture, ImageSize size)
		{
			if (string.IsNullOrEmpty(_staticRoot))
				return string.Empty;
			var relative = $@"img\{picture.EntryType}\main{GetDir(size)}\{picture.Id}{ImageHelper.GetExtensionFromMime(picture.Mime)}";
			return Path.Combine(_staticRoot, relative);
		}

		public ServerEntryThumbPersister()
		{
			_staticRoot = AppConfig.StaticContentPath;
		}

		public override VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size) => new VocaDbUrl(GetRelativeUrl(picture, size), UrlDomain.Static, UriKind.Relative);

		public override bool IsSupported(IEntryImageInformation picture, ImageSize size) => picture.EntryType == EntryType.ReleaseEvent || picture.EntryType == EntryType.ReleaseEventSeries || picture.EntryType == EntryType.SongList || picture.EntryType == EntryType.Tag
			|| ((picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album)
				&& picture.PurposeMainOrUnspecified()
				&& size != ImageSize.Original);
	}
}
