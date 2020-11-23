namespace VocaDb.Model.Domain
{
	public interface IEntryBase : IDeletableEntry
	{
		string DefaultName { get; }

		EntryType EntryType { get; }

		/// <summary>
		/// Current entry version number.
		/// If the entry doesn't support versioning, this should always be 0.
		/// </summary>
		int Version { get; }
	}
}
