using System;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using NLog;
using TagLib;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;
using File = System.IO.File;

namespace VocaDb.Model.Service.VideoServices {

	public class LocalFileManager {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		public const int MaxMediaSizeMB = 20;
		public const int MaxMediaSizeBytes = MaxMediaSizeMB * 1024 * 1024;
		public static readonly string[] Extensions = { ".mp3", ".jpg", ".png" };
		public static readonly string[] MimeTypes = { "audio/mp3", "audio/mpeg", "image/jpeg", "image/png" };

		public static bool IsAudio(string filename) {
			string[] imageExtensions = { ".jpg", ".png" };
			var ext = Path.GetExtension(filename);
			return imageExtensions.Contains(ext);
		}


		public PVContract CreatePVContract(HttpPostedFileBase file, IIdentity user, IUser loggedInUser) {

			var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ImageHelper.GetExtensionFromMime(file.ContentType));
			file.SaveAs(tempFile);

			var filename = Path.GetFileName(tempFile);
			var pv = new PVContract { Service = PVService.LocalFile, PVId = filename };

			using (var mp3 = TagLib.File.Create(tempFile, file.ContentType, ReadStyle.Average)) {
				pv.Name = mp3.Tag.Title;
				pv.Author = user.Name;
				pv.Length = (int)mp3.Properties.Duration.TotalSeconds;
			}

			pv.CreatedBy = loggedInUser.Id;

			if (string.IsNullOrEmpty(pv.Name))
				pv.Name = Path.GetFileNameWithoutExtension(file.FileName);

			return pv;

		}

		private string GetFilesystemPath(string pvId) {
			return Path.Combine(AppConfig.StaticContentPath + "\\media\\", pvId);
        }

		public void SyncLocalFilePVs(CollectionDiff<PVForSong, PVForSong> diff, int songId) {

			var addedLocalMedia = diff.Added.Where(m => m.Service == PVService.LocalFile);
			foreach (var pv in addedLocalMedia) {

				var oldFull = Path.Combine(Path.GetTempPath(), pv.PVId);

				if (Path.GetDirectoryName(oldFull) != Path.GetDirectoryName(Path.GetTempPath()))
					throw new InvalidOperationException("File folder doesn't match with temporary folder");

				if (!Extensions.Contains(Path.GetExtension(oldFull)))
					throw new InvalidOperationException("Invalid extension");

				var newId = string.Format("{0}-S{1}-{2}", pv.Author, songId, pv.PVId);
				var newFull = GetFilesystemPath(newId);
				pv.PVId = newId;

				try {

					File.Move(oldFull, newFull);

					// Remove copied permissions, reset to inherited http://stackoverflow.com/a/2930969
					var fs = File.GetAccessControl(newFull);
					fs.SetAccessRuleProtection(false, false);
					File.SetAccessControl(newFull, fs);

				} catch (IOException x) {
					log.Error(x, "Unable to move local media file: " + oldFull);
					throw;
				}

			}

			foreach (var pv in diff.Removed.Where(m => m.Service == PVService.LocalFile)) {
				var fullPath = GetFilesystemPath(pv.PVId);
				if (File.Exists(fullPath)) {
					try {
						File.Delete(fullPath);
					} catch (IOException x) {
						log.Error(x, "Unable to delete local media file: " + fullPath);
					}
                }
			}

		}

	}

}
