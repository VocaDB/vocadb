#nullable disable

using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.Domain
{
	public class EntryThumb : IEntryImageInformation
	{
#nullable enable
		public static EntryThumb? Create<T>(T? entry) where T : class, IEntryBase, IEntryImageInformation
		{
			return !string.IsNullOrEmpty(entry?.Mime) ? new EntryThumb(entry, entry.Mime, entry.Purpose) : null;
		}
#nullable disable

		private IEntryBase _entry;

		public EntryThumb() { }

		public EntryThumb(IEntryBase entry, string mime, ImagePurpose purpose)
		{
			Entry = entry;
			Mime = mime;
			Purpose = purpose;
		}

		public IEntryBase Entry
		{
			get => _entry;
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

		public EntryType EntryType => Entry.EntryType;

		public int Id => Entry.Id;

		public string Mime { get; set; }

		public ImagePurpose Purpose { get; set; }

#nullable enable
		public override string ToString()
		{
			return $"Thumbnail for {Entry}.";
		}
#nullable disable

		public int Version => Entry.Version;
	}

	public class EntryThumbMain : EntryThumb
	{
		public EntryThumbMain()
		{
			Purpose = ImagePurpose.Main;
		}

		public EntryThumbMain(IEntryBase entry, string mime)
			: base(entry, mime, ImagePurpose.Main) { }
	}
}
