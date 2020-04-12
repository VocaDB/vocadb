using System.Drawing;
using System.IO;

namespace VocaDb.Model.Domain.Images {

	public abstract class ServerEntryImagePersisterBase : IEntryImagePersister {

		private void EnsureDirExistsForFile(string path) {

			var dir = Path.GetDirectoryName(path);

			if (dir != null && !Directory.Exists(dir))
				Directory.CreateDirectory(dir);

		}

		public Stream GetReadStream(IEntryImageInformation picture, ImageSize size) {
			return File.OpenRead(GetPath(picture, size));
		}

		public abstract VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size);

		public bool HasImage(IEntryImageInformation picture, ImageSize size) {
			return File.Exists(GetPath(picture, size));
		}

		public abstract string GetPath(IEntryImageInformation picture, ImageSize size);

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

		public abstract bool IsSupported(IEntryImageInformation picture, ImageSize size);

	}

}
