using System;
using System.Linq;

namespace VocaDb.Model.Domain.Versioning {

	public abstract class EntryDiff<T> : IEntryDiff where T : struct, IConvertible {

		protected EnumFieldAccessor<T> Field(T field) {
			return new EnumFieldAccessor<T>(ChangedFields, field);
		}

		private bool IsDefault(T val) {
			return val.Equals(default(T));
		}

		protected EntryDiff(bool isSnapshot = false) {
			IsSnapshot = isSnapshot;
			ChangedFields = new EnumVal<T>();
		} 

		public virtual EnumVal<T> ChangedFields { get; set; }

		public virtual string[] ChangedFieldNames {
			get {

				var fieldNames = EnumVal<T>.Values
					.Where(f => !IsDefault(f) && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();

			}
		}

		public virtual string ChangedFieldsString {
			get {

				var fieldNames = EnumVal<T>.Values.Where(f => !IsDefault(f) && IsChanged(f));
				return string.Join(",", fieldNames);

			}
			set {

				ChangedFields.Clear();

				if (string.IsNullOrEmpty(value)) {
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames) {
					T field;
					if (Enum.TryParse(name, out field))
						SetChanged(field);
				}

			}
		}

		public virtual bool IsSnapshot { get; set; }

		/// <summary>
		/// Checks whether a specific field changed in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field was changed, otherwise false.</returns>
		public bool IsChanged(T field) {
			return ChangedFields.FlagIsSet(field);
		}

		public void SetChanged(T field) {
			ChangedFields.SetFlag(field, true);			
		}

	}

	public struct EnumFieldAccessor<T> where T : struct, IConvertible {

		private readonly EnumVal<T> val; 
		private readonly T field;

		public EnumFieldAccessor(EnumVal<T> val, T field)
			: this() {
			this.val = val;
			this.field = field;
		}

		public void Set() {
			val.SetFlag(field, true);
		}

	}

}
