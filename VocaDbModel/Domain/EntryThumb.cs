using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain {

	public class EntryThumb : IPictureWithThumbs, IEntryImageInformation {

		private static string GetExtension(string mime) {
			return ImageHelper.GetExtensionFromMime(mime) ?? string.Empty;
		}

		private static string GetFileName(int id, string mime, string suffix) {
			return string.Format("{0}{1}{2}", id, suffix, GetExtension(mime));
		}

		private IEntryBase entry;

		public EntryThumb() {}

		public EntryThumb(IEntryBase entry, string mime) {
			Entry = entry;
			Mime = mime;
		}

		public IEntryBase Entry {
			get { return entry; }
			set {
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public EntryType EntryType {
			get { return Entry.EntryType; }
		}

		public virtual string FileName {
			get {
				return GetFileName(Entry.Id, Mime, string.Empty);
			}
		}

		public virtual string FileNameThumb {
			get {
				return GetFileName(Entry.Id, Mime, "-t");
			}
		}

		public virtual string FileNameSmallThumb {
			get {
				return GetFileName(Entry.Id, Mime, "-st");
			}
		}

		public virtual string FileNameTinyThumb {
			get {
				return GetFileName(Entry.Id, Mime, "-tt");
			}
		}

		public int Id {
			get { return Entry.Id; }
		}

		public string Mime { get; set; }

		public override string ToString() {
			return string.Format("Thumbnail for {0}.", Entry);
		}

		public int Version {
			get { return Entry.Version; }
		}

	}
}
