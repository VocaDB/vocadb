using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;

namespace VocaDb.Model.Service.VideoServices {

	public class LocalFileManager {

		private string GetFilesystemPath(string pvId) {
			return Path.Combine(AppConfig.StaticContentPath + "\\media\\", pvId);
        }

		public void SyncLocalFilePVs(CollectionDiff<PVForSong, PVForSong> diff, int songId) {

			var addedLocalMedia = diff.Added.Where(m => m.Service == PVService.LocalFile);
			foreach (var pv in addedLocalMedia) {

				var oldFull = Path.Combine(Path.GetTempPath(), pv.PVId);

				if (Path.GetDirectoryName(oldFull) != Path.GetDirectoryName(Path.GetTempPath()))
					throw new InvalidOperationException("File folder doesn't match with temporary folder");

				if (Path.GetExtension(oldFull) != ".mp3")
					throw new InvalidOperationException("Invalid extension");

				var newId = string.Format("{0}-{1}-{2}", pv.Author, songId, pv.PVId);
				var newFull = GetFilesystemPath(newId);
				pv.PVId = newId;

				File.Move(oldFull, newFull);

			}

			foreach (var pv in diff.Removed.Where(m => m.Service == PVService.LocalFile)) {
				var fullPath = GetFilesystemPath(pv.PVId);
				if (File.Exists(fullPath))
					File.Delete(fullPath);
			}

		}

	}

}
