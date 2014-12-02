using System.Linq;
using System;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumDiff : IEntryDiff {

		private void Set(AlbumEditableFields field, bool val) {

			if (val && !IsChanged(field))
				ChangedFields |= field;
			else if (!val && IsChanged(field))
				ChangedFields -= field;

		}

		public AlbumDiff()
			: this(true) { }

		public AlbumDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Artists {
			get {
				return IsChanged(AlbumEditableFields.Artists);
			}
			set {
				Set(AlbumEditableFields.Artists, value);
			}
		}

		public virtual string[] ChangedFieldNames {
			get {

				var fieldNames = EnumVal<AlbumEditableFields>.Values
					.Where(f => f != AlbumEditableFields.Nothing && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();

			}
		}

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<AlbumEditableFields>.Values.Where(f => f != AlbumEditableFields.Nothing && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = AlbumEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					AlbumEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual AlbumEditableFields ChangedFields { get; set; }

		public virtual bool Cover {
			get {
				return IsChanged(AlbumEditableFields.Cover);
			}
			set {
				Set(AlbumEditableFields.Cover, value);
			}
		}

		public virtual bool Description {
			get {
				return IsChanged(AlbumEditableFields.Description);
			}
			set {
				Set(AlbumEditableFields.Description, value);
			}		
		}

		public virtual bool DiscType {
			get {
				return IsChanged(AlbumEditableFields.DiscType);
			}
			set {
				Set(AlbumEditableFields.DiscType, value);
			}
		}

		public virtual bool Identifiers {
			get {
				return IsChanged(AlbumEditableFields.Identifiers);
			}
			set {
				Set(AlbumEditableFields.Identifiers, value);
			}
		}

		public virtual bool IncludeArtists {
			get {
				return (IsSnapshot || Artists);
			}
		}

		public virtual bool IncludeCover {
			get {
				// Special treatment for cover - not included in snapshots by default.
				return Cover;
			}
		}

		public virtual bool IncludeDescription {
			get {
				return (IsSnapshot || Description);
			}
		}

		public virtual bool IncludeNames {
			get {
				return (IsSnapshot || Names);
			}
		}

		public virtual bool IncludePictures {
			get {
				return (IsSnapshot || Pictures);
			}
		}

		public virtual bool IncludePVs {
			get {
				return (IsSnapshot || PVs);
			}
		}

		public virtual bool IncludeTracks {
			get {
				return (IsSnapshot || Tracks);
			}
		}

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Names {
			get {
				return IsChanged(AlbumEditableFields.Names);
			}
			set {
				Set(AlbumEditableFields.Names, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsChanged(AlbumEditableFields.OriginalName);
			}
			set {
				Set(AlbumEditableFields.OriginalName, value);
			}
		}

		public virtual bool OriginalRelease {
			get {
				return IsChanged(AlbumEditableFields.OriginalRelease);
			}
			set {
				Set(AlbumEditableFields.OriginalRelease, value);
			}
		}

		public virtual bool Pictures {
			get {
				return IsChanged(AlbumEditableFields.Pictures);
			}
			set {
				Set(AlbumEditableFields.Pictures, value);
			}
		}

		public virtual bool PVs {
			get {
				return IsChanged(AlbumEditableFields.PVs);
			}
			set {
				Set(AlbumEditableFields.PVs, value);
			}
		}

		public virtual bool Status {
			get {
				return IsChanged(AlbumEditableFields.Status);
			}
			set {
				Set(AlbumEditableFields.Status, value);
			}
		}

		public virtual bool Tracks {
			get {
				return IsChanged(AlbumEditableFields.Tracks);
			}
			set {
				Set(AlbumEditableFields.Tracks, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsChanged(AlbumEditableFields.WebLinks);
			}
			set {
				Set(AlbumEditableFields.WebLinks, value);
			}		
		}

		/// <summary>
		/// Checks whether a specific field changed in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field was changed, otherwise false.</returns>
		public bool IsChanged(AlbumEditableFields field) {
			return ChangedFields.HasFlag(field);
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
		public bool IsIncluded(AlbumEditableFields field) {

			return (field != AlbumEditableFields.Cover ? (IsSnapshot || IsChanged(field)) : IncludeCover);

		}

	}


}
