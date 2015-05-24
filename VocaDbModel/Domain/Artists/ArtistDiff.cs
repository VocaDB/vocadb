using System;
using System.Linq;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistDiff : IEntryDiff {

		private bool IsChanged(ArtistEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

		private void Set(ArtistEditableFields field, bool val) {

			if (val && !IsChanged(field))
				ChangedFields |= field;
			else if (!val && IsChanged(field))
				ChangedFields -= field;

		}

		public ArtistDiff()
			: this(true) { }

		public ArtistDiff(bool isSnapshot) {
			IsSnapshot = isSnapshot;
		}

		public virtual bool Albums {
			get {
				return IsChanged(ArtistEditableFields.Albums);
			}
			set {
				Set(ArtistEditableFields.Albums, value);
			}
		}

		public virtual bool ArtistType {
			get {
				return IsChanged(ArtistEditableFields.ArtistType);
			}
			set {
				Set(ArtistEditableFields.ArtistType, value);
			}
		}

		public virtual bool BaseVoicebank {
			get {
				return IsChanged(ArtistEditableFields.BaseVoicebank);
			}
			set {
				Set(ArtistEditableFields.BaseVoicebank, value);
			}
		}

		public virtual string[] ChangedFieldNames {
			get {

				var fieldNames = EnumVal<ArtistEditableFields>.Values
					.Where(f => f != ArtistEditableFields.Nothing && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();

			}
		}

		public virtual ArtistEditableFields ChangedFields { get; set; }

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<ArtistEditableFields>.Values.Where(f => f != ArtistEditableFields.Nothing && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = ArtistEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					ArtistEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual bool Description {
			get {
				return IsChanged(ArtistEditableFields.Description);
			}
			set {
				Set(ArtistEditableFields.Description, value);
			}
		}

		public virtual bool Groups {
			get {
				return IsChanged(ArtistEditableFields.Groups);
			}
			set {
				Set(ArtistEditableFields.Groups, value);
			}
		}

		public virtual bool IncludeAlbums {
			get {
				// Note: albums list not included anymore.
				return Albums;
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

		public virtual bool IncludeWebLinks {
			get {
				return (IsSnapshot || WebLinks);
			}
		}

		public bool IncludePicture {
			get {
				// Special handling for pictures
				return Picture;
			}
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Names {
			get {
				return IsChanged(ArtistEditableFields.Names);
			}
			set {
				Set(ArtistEditableFields.Names, value);
			}
		}

		public virtual bool OriginalName {
			get {
				return IsChanged(ArtistEditableFields.OriginalName);
			}
			set {
				Set(ArtistEditableFields.OriginalName, value);
			}
		}

		public virtual bool Picture {
			get {
				return IsChanged(ArtistEditableFields.Picture);
			}
			set {
				Set(ArtistEditableFields.Picture, value);
			}
		}

		public virtual bool Pictures {
			get {
				return IsChanged(ArtistEditableFields.Pictures);
			}
			set {
				Set(ArtistEditableFields.Pictures, value);
			}
		}
		public virtual bool Status {
			get {
				return IsChanged(ArtistEditableFields.Status);
			}
			set {
				Set(ArtistEditableFields.Status, value);
			}
		}

		public virtual bool WebLinks {
			get {
				return IsChanged(ArtistEditableFields.WebLinks);
			}
			set {
				Set(ArtistEditableFields.WebLinks, value);
			}
		}

		/// <summary>
		/// Checks whether a specific field is included in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field is included, otherwise false.</returns>
		/// <remarks>
		/// Snapshots include all fields except the Cover.
		/// Other fields are commonly included only they are changed.
		/// </remarks>
		public bool IsIncluded(ArtistEditableFields field) {

			return (field != ArtistEditableFields.Picture ? (IsSnapshot || IsChanged(field)) : IncludePicture);

		}

	}

}
