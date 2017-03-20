using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.Domain {

	public class EntryThumb : IEntryImageInformation {

		public static EntryThumb Create<T>(T entry) where T : IEntryBase, IEntryImageInformation {
			return new EntryThumb(entry, entry.Mime);
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
