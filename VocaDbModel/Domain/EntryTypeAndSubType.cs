namespace VocaDb.Model.Domain {

	public readonly struct EntryTypeAndSubType {

		public EntryTypeAndSubType(EntryType entryType, string subType = "") {
			EntryType = entryType;
			SubType = subType;
		}

		public EntryType EntryType { get; }
		public bool HasValue => EntryType != EntryType.Undefined;
		public bool HasSubType => !string.IsNullOrEmpty(SubType);
		public string SubType { get; }

	}
}
