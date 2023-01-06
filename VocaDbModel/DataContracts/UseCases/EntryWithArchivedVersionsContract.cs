namespace VocaDb.Model.DataContracts.UseCases;

[Obsolete]
public class EntryWithArchivedVersionsContract<TEntry, TVersion> : IEntryWithArchivedVersionsContract<TEntry, TVersion>
{
#nullable disable
	public EntryWithArchivedVersionsContract() { }
#nullable enable

	public EntryWithArchivedVersionsContract(TEntry entry, TVersion[] archivedVersions)
	{
		ArchivedVersions = archivedVersions;
		Entry = entry;
	}

	public TVersion[] ArchivedVersions { get; init; }

	public TEntry Entry { get; init; }
}

public interface IEntryWithArchivedVersionsContract<TEntry, out TVersion>
{
	TVersion[] ArchivedVersions { get; }

	TEntry Entry { get; }
}

[Obsolete]
public static class EntryWithArchivedVersionsContract
{
	public static EntryWithArchivedVersionsContract<TEntry, TVersion> Create<TEntry, TVersion>(TEntry entry, TVersion[] versions)
	{
		return new EntryWithArchivedVersionsContract<TEntry, TVersion>(entry, versions);
	}
}
