#nullable disable

using System;
using System.Linq;

namespace VocaDb.Model.Domain.Versioning
{
	public abstract class EntryDiff<T> : IEntryDiff where T : struct, Enum
	{
		protected EnumFieldAccessor<T> Field(T field)
		{
			return new EnumFieldAccessor<T>(ChangedFields, field);
		}

		private bool IsDefault(T val)
		{
			return val.Equals(default(T));
		}

		protected EntryDiff(bool isSnapshot = false)
		{
			IsSnapshot = isSnapshot;
			ChangedFields = new EnumVal<T>();
		}

		protected EntryDiff(T changedFields)
		{
			ChangedFields = new EnumVal<T>(changedFields);
			IsSnapshot = false;
		}

		public virtual EnumVal<T> ChangedFields { get; set; }

		public virtual string[] ChangedFieldNames
		{
			get
			{
				var fieldNames = EnumVal<T>.Values
					.Where(f => !IsDefault(f) && IsChanged(f)).Select(f => f.ToString());

				return fieldNames.ToArray();
			}
		}

		public virtual string ChangedFieldsString
		{
			get
			{
				var fieldNames = EnumVal<T>.Values.Where(f => !IsDefault(f) && IsChanged(f));
				return string.Join(",", fieldNames);
			}
			set
			{
				ChangedFields.Clear();

				if (string.IsNullOrEmpty(value))
				{
					return;
				}

				var fieldNames = value.Split(',');
				foreach (var name in fieldNames)
				{
					if (Enum.TryParse(name, out T field))
						SetChanged(field);
				}
			}
		}

		/// <summary>
		/// Whether the current version is a snapshot.
		/// Most fields are included with the snapshot.
		/// </summary>
		public virtual bool IsSnapshot { get; set; }

		/// <summary>
		/// Checks whether a specific field changed in this diff.
		/// </summary>
		/// <param name="field">Field to be checked.</param>
		/// <returns>True if the field was changed, otherwise false.</returns>
		public bool IsChanged(T field)
		{
			return ChangedFields.FlagIsSet(field);
		}

		/// <summary>
		/// Whether a specific field is included in this diff.
		/// Generally, for non-snapshot diffs, fields that are changed are included.
		/// For snapshots most fields are included.
		/// </summary>
		/// <param name="field">Field to be tested.</param>
		/// <returns>True if the field should be included in this diff.</returns>
		public virtual bool IsIncluded(T field)
		{
			return IsSnapshot || IsChanged(field);
		}

		public void SetChanged(T field)
		{
			ChangedFields.SetFlag(field, true);
		}
	}

	public struct EnumFieldAccessor<T> where T : struct, Enum
	{
		private readonly EnumVal<T> _val;
		private readonly T _field;

		public EnumFieldAccessor(EnumVal<T> val, T field)
			: this()
		{
			_val = val;
			_field = field;
		}

		public bool IsChanged
		{
			get => _val.FlagIsSet(_field);
			set => _val.SetFlag(_field, value);
		}

		public void Set(bool value = true)
		{
			_val.SetFlag(_field, value);
		}

#nullable enable
		public override string ToString()
		{
			return $"{_field} changed: {IsChanged}";
		}
#nullable disable
	}
}
