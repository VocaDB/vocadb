namespace VocaDb.Model.Domain {

	/// <summary>
	/// Entry identifier that is a combination of entry type and primary key ID.
	/// This combination is unique site-wide.
	/// </summary>
	public struct GlobalEntryId {

		public static readonly GlobalEntryId Empty = new GlobalEntryId();

		public GlobalEntryId(EntryType entryType, int id) 
			: this() {

			EntryType = entryType;
			Id = id;

		}

		public EntryType EntryType { get; private set; }

		public int Id { get; private set; }

		public bool IsEmpty {
			get { return Id == 0; }
		}

		public override string ToString() {
			return string.Format("{0}.{1}", EntryType, Id);
		}

		public bool Equals(GlobalEntryId other) {
			return EntryType == other.EntryType && Id == other.Id;
		}

		public override bool Equals(object obj) {
			if (ReferenceEquals(null, obj)) return false;
			return obj is GlobalEntryId && Equals((GlobalEntryId) obj);
		}

		public override int GetHashCode() {
			unchecked {
				return ((int) EntryType*397) ^ Id;
			}
		}

		public static bool operator ==(GlobalEntryId left, GlobalEntryId right) {
			return left.Equals(right);
		}

		public static bool operator !=(GlobalEntryId left, GlobalEntryId right) {
			return !left.Equals(right);
		}

	}
}
