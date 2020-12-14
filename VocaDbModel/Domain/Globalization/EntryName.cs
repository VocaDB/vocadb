#nullable disable


namespace VocaDb.Model.Domain.Globalization
{
	public class EntryName<TEntry> : LocalizedStringWithId where TEntry : class
	{
		private TEntry entry;

		public EntryName() { }

		public EntryName(TEntry entry, ILocalizedString localizedString)
			: base(localizedString.Value, localizedString.Language)
		{
			Entry = entry;
		}

		public virtual TEntry Entry
		{
			get { return entry; }
			set
			{
				ParamIs.NotNull(() => value);
				entry = value;
			}
		}

		public virtual bool Equals(EntryName<TEntry> another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as EntryName<TEntry>);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		public override string ToString()
		{
			return string.Format("name '{0}' for {1}", Value, Entry);
		}
	}
}
