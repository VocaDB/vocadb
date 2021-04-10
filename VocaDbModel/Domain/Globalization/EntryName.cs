#nullable disable


namespace VocaDb.Model.Domain.Globalization
{
	public class EntryName<TEntry> : LocalizedStringWithId where TEntry : class
	{
		private TEntry _entry;

		public EntryName() { }

		public EntryName(TEntry entry, ILocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get => _entry;
			set
			{
				ParamIs.NotNull(() => value);
				_entry = value;
			}
		}

#nullable enable
		public virtual bool Equals(EntryName<TEntry>? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as EntryName<TEntry>);
		}
#nullable disable

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return $"name '{Value}' for {Entry}";
		}
	}
}
