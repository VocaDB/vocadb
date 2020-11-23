namespace VocaDb.Model.Domain
{

	/// <summary>
	/// Entry identifier that is a combination of entry type and primary key ID.
	/// This combination is unique site-wide.
	/// </summary>
	public readonly struct GlobalEntryId
	{

		public static readonly GlobalEntryId Empty = new GlobalEntryId();

		public GlobalEntryId(EntryType entryType, int id)
			: this()
		{

			EntryType = entryType;
			Id = id;

		}

		public EntryType EntryType { get; }

		public int Id { get; }

		public bool IsEmpty => Id == 0;

		public override string ToString()
		{
			return string.Format("{0}.{1}", EntryType, Id);
		}

		public bool Equals(GlobalEntryId other)
		{
			return EntryType == other.EntryType && Id == other.Id;
		}

		public override bool Equals(object obj)
		{
			if (obj is null) return false;
			return obj is GlobalEntryId && Equals((GlobalEntryId)obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((int)EntryType * 397) ^ Id;
			}
		}

		public static bool operator ==(GlobalEntryId left, GlobalEntryId right) => left.Equals(right);

		public static bool operator !=(GlobalEntryId left, GlobalEntryId right) => !left.Equals(right);

	}
}
