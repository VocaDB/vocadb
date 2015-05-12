using System;
using System.Linq;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class SongDiff : IEntryDiff {

		private bool IsChanged(SongEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

		private void Set(SongEditableFields field, bool val) {

			if (val && !IsChanged(field))
				ChangedFields |= field;
			else if (!val && IsChanged(field))
				ChangedFields -= field;

		}

		public SongDiff()
			: this(true) { }

		public SongDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Artists {
			get {
				return IsChanged(SongEditableFields.Artists);
			}
			set {
				Set(SongEditableFields.Artists, value);
			}
		}

		public virtual SongEditableFields ChangedFields { get; set; }

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<SongEditableFields>.Values.Where(f => f != SongEditableFields.Nothing && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = SongEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					SongEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual bool IncludeArtists {
			get {
				return (IsSnapshot || Artists);
			}
		}

		public bool IncludeLyrics {
			get {
				return (IsSnapshot || Lyrics);
			}
		}

		public virtual bool IncludeNames {
			get {
				return (IsSnapshot || Names);
			}
		}

		public virtual bool IncludePVs {
			get {
				return (IsSnapshot || PVs);
			}
		}

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Length {
			get {
				return IsChanged(SongEditableFields.Length);
			}
			set {
				Set(SongEditableFields.Length, value);
			}
		}

		public virtual bool Lyrics {
			get {
				return IsChanged(SongEditableFields.Lyrics);
			}
			set {
				Set(SongEditableFields.Lyrics, value);
			}
		}

		public virtual bool Names {
			get {
				return IsChanged(SongEditableFields.Names);
			}
			set {
				Set(SongEditableFields.Names, value);
			}
		}

		public virtual bool Notes {
			get {
				return IsChanged(SongEditableFields.Notes);
			}
			set {
				Set(SongEditableFields.Notes, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsChanged(SongEditableFields.OriginalName);
			}
			set {
				Set(SongEditableFields.OriginalName, value);
			}
		}

		public virtual bool OriginalVersion {
			get {
				return IsChanged(SongEditableFields.OriginalVersion);
			}
			set {
				Set(SongEditableFields.OriginalVersion, value);
			}
		}

		public virtual bool PublishDate {
			get {
				return IsChanged(SongEditableFields.PublishDate);
			}
			set {
				Set(SongEditableFields.PublishDate, value);
			}
		}

		public virtual bool PVs {
			get {
				return IsChanged(SongEditableFields.PVs);
			}
			set {
				Set(SongEditableFields.PVs, value);
			}
		}

		public virtual bool SongType {
			get {
				return IsChanged(SongEditableFields.SongType);
			}
			set {
				Set(SongEditableFields.SongType, value);
			}
		}

		public virtual bool Status {
			get {
				return IsChanged(SongEditableFields.Status);
			}
			set {
				Set(SongEditableFields.Status, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsChanged(SongEditableFields.WebLinks);
			}
			set {
				Set(SongEditableFields.WebLinks, value);
			}
		}

		/// <summary>
		/// Checks whether a specific field is included in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field is included, otherwise false.</returns><
		/// <remarks>
		/// Snapshots include all fields except the Cover.
		/// Other fields are commonly included only they are changed.
		/// </remarks>
		public bool IsIncluded(SongEditableFields field) {

			return (IsSnapshot || IsChanged(field));

		}

	}

}
