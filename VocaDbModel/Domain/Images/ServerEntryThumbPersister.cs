using System;
using System.Drawing;
using System.IO;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Saves entry thumbnails (for album/artist) in the static files folder on the server disk.
	/// </summary>
	public class ServerEntryThumbPersister : IEntryThumbPersister {

		private readonly string staticRoot;

		private void EnsureDirExistsForFile(string path) {

			var dir = Path.GetDirectoryName(path);

			if (dir != null && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

		}

		private static string GetDir(ImageSize size) {

			switch (size) {
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

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) {
			return File.OpenRead(GetPath(picture, size));
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return File.Exists(GetPath(picture, size));
		}

		private string GetRelativeUrl(IEntryImageInformation picture, ImageSize size) {
			if (picture.Version > 0) {
				return string.Format("/img/{0}/main{1}/{2}{3}?v={4}", picture.EntryType.ToString().ToLowerInvariant(), GetDir(size), picture.Id, 
					ImageHelper.GetExtensionFromMime(picture.Mime), picture.Version);
			} else
				return string.Format("/img/{0}/main{1}/{2}{3}", picture.EntryType.ToString().ToLowerInvariant(), GetDir(size), picture.Id, ImageHelper.GetExtensionFromMime(picture.Mime));
		}

		private string GetPath(IEntryImageInformation picture, ImageSize size) {
			var relative = string.Format(@"img\{0}\main{1}\{2}{3}", picture.EntryType, GetDir(size), picture.Id, ImageHelper.GetExtensionFromMime(picture.Mime));
			return Path.Combine(staticRoot, relative);
		}

		public ServerEntryThumbPersister() {
			
			staticRoot = AppConfig.StaticContentPath;

		}

		public string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size, bool ssl) {
			return VocaUriBuilder.StaticResource(GetRelativeUrl(picture, size), ssl);
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Stream file) {
			
			var path = GetPath(picture, size);

			if (string.IsNullOrEmpty(path))
				return;

			EnsureDirExistsForFile(path);

			file.Seek(0, SeekOrigin.Begin);

			using (var f = File.Create(path)) {
				file.CopyTo(f);
			}

			file.Seek(0, SeekOrigin.Begin);
				
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Image image) {
			
			var path = GetPath(picture, size);

			if (string.IsNullOrEmpty(path))
				return;

			EnsureDirExistsForFile(path);

			image.Save(path);	

		}
	}

}
