using VocaDb.Model.DataContracts.Versioning;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class EntryWithArchivedVersionsForApiContract<TEntry> : IEntryWithArchivedVersionsForApiContract<TEntry>
	{
#nullable disable
		public EntryWithArchivedVersionsForApiContract() { }
#nullable enable

		public EntryWithArchivedVersionsForApiContract(TEntry entry, ArchivedObjectVersionForApiContract[] archivedVersions)
		{
			ArchivedVersions = archivedVersions;
			Entry = entry;
		}

		public ArchivedObjectVersionForApiContract[] ArchivedVersions { get; init; }

		public TEntry Entry { get; init; }
	}

	public interface IEntryWithArchivedVersionsForApiContract<TEntry>
	{
		ArchivedObjectVersionForApiContract[] ArchivedVersions { get; }

		TEntry Entry { get; }
	}

	public static class EntryWithArchivedVersionsForApiContract
	{
		public static EntryWithArchivedVersionsForApiContract<TEntry> Create<TEntry>(TEntry entry, ArchivedObjectVersionForApiContract[] versions)
		{
			return new EntryWithArchivedVersionsForApiContract<TEntry>(entry, versions);
		}
	}
}
