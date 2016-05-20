using System;
using System.Linq;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEventDiff : IEntryDiff {

		private void Set(ReleaseEventEditableFields field, bool val) {

			if (val && !IsChanged(field))
				ChangedFields |= field;
			else if (!val && IsChanged(field))
				ChangedFields -= field;

		}

		public ReleaseEventDiff() {
			IsSnapshot = true;
		}

		public virtual string[] ChangedFieldNames {
			get {

				var fieldNames = EnumVal<ReleaseEventEditableFields>.Values
					.Where(f => f != ReleaseEventEditableFields.Nothing && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();

			}
		}

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<ReleaseEventEditableFields>.Values.Where(f => f != ReleaseEventEditableFields.Nothing && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields = ReleaseEventEditableFields.Nothing;

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					ReleaseEventEditableFields field;
					if (Enum.TryParse(name, out field))
						Set(field, true);
				}

			}
		}

		public virtual ReleaseEventEditableFields ChangedFields { get; set; }

		public virtual bool Date {
			get {
				return IsChanged(ReleaseEventEditableFields.Date);
			}
			set {
				Set(ReleaseEventEditableFields.Date, value);
			}
		}

		public virtual bool Description {
			get {
				return IsChanged(ReleaseEventEditableFields.Description);
			}
			set {
				Set(ReleaseEventEditableFields.Description, value);
			}		
		}

		public virtual bool IsSnapshot { get; set; }

		public virtual bool Name {
			get {
				return IsChanged(ReleaseEventEditableFields.Name);
			}
			set {
				Set(ReleaseEventEditableFields.Name, value);
			}
		}

		public virtual bool Series {
			get {
				return IsChanged(ReleaseEventEditableFields.Series);
			}
			set {
				Set(ReleaseEventEditableFields.Series, value);
			}
		}

		public virtual bool SeriesNumber {
			get {
				return IsChanged(ReleaseEventEditableFields.SeriesNumber);
			}
			set {
				Set(ReleaseEventEditableFields.SeriesNumber, value);
			}
		}

		public virtual bool SeriesSuffix {
			get {
				return IsChanged(ReleaseEventEditableFields.SeriesSuffix);
			}
			set {
				Set(ReleaseEventEditableFields.SeriesSuffix, value);
			}
		}

		/// <summary>
		/// Checks whether a specific field changed in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field was changed, otherwise false.</returns>
		public bool IsChanged(ReleaseEventEditableFields field) {
			return ChangedFields.HasFlag(field);
		}

	}

}
