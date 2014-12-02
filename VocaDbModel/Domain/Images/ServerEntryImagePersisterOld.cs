using System.Drawing;
using System.IO;
using System.Web;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Domain.Images {

	/// <summary>
	/// Saves songlist/tag thumbnails on the server disk.
	/// 
	/// These images are saved in the EntryImg folder under the main application folder for now,
	/// but should be migrated to the static files folder.
	/// Hence the "Old" suffix.
	/// </summary>
	public class ServerEntryImagePersisterOld : IEntryImagePersisterOld {

		private void EnsureDirExistsForFile(string path) {

			var dir = Path.GetDirectoryName(path);

			if (dir != null && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

		}

		private static string GetFileName(int id, string mime, string suffix) {
			return string.Format("{0}{1}{2}", id, suffix, ImageHelper.GetExtensionFromMime(mime));
		}

		private static string GetFileName(IEntryImageInformation picture, ImageSize size) {
			return GetFileName(picture.Id, picture.Mime, GetSuffix(size));
		}

		private static string GetSuffix(ImageSize size) {
			switch (size) {
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

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) {
			return File.OpenRead(GetPath(picture, size));
		}

		public string GetUrlAbsolute(IEntryImageInformation picture, ImageSize size, bool ssl) {
			var host = ssl ? AppConfig.StaticContentHostSSL : AppConfig.HostAddress;
			return string.Format("{0}/EntryImg/{1}/{2}", host, picture.EntryType, GetFileName(picture, size));
		}

		public string GetPath(IEntryImageInformation picture, ImageSize size) {
			return HttpContext.Current.Server.MapPath(string.Format("~\\EntryImg\\{0}\\{1}", picture.EntryType, GetFileName(picture, size)));
		}

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return File.Exists(GetPath(picture, size));
		}

		public void Write(IEntryImageInformation picture, ImageSize size, Image image) {
			
			var path = GetPath(picture, size);

			if (string.IsNullOrEmpty(path))
				return;

			EnsureDirExistsForFile(path);

			image.Save(path);					

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
	}
}
