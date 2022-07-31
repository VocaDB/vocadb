using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Versioning;

public interface IEntryWithArchivedVersionsForApiContract<TEntry>
{
	ArchivedObjectVersionForApiContract[] ArchivedVersions { get; }

	TEntry Entry { get; }
}

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record EntryWithArchivedVersionsForApiContract<TEntry> : IEntryWithArchivedVersionsForApiContract<TEntry>
{
#nullable disable
	public EntryWithArchivedVersionsForApiContract() { }
#nullable enable

	public EntryWithArchivedVersionsForApiContract(TEntry entry, ArchivedObjectVersionForApiContract[] archivedVersions)
	{
		ArchivedVersions = archivedVersions;
		Entry = entry;
	}

	[DataMember]
	public ArchivedObjectVersionForApiContract[] ArchivedVersions { get; init; }

	[DataMember]
	public TEntry Entry { get; init; }
}

public static class EntryWithArchivedVersionsForApiContract
{
	public static EntryWithArchivedVersionsForApiContract<TEntry> Create<TEntry>(TEntry entry, ArchivedObjectVersionForApiContract[] versions)
	{
		return new EntryWithArchivedVersionsForApiContract<TEntry>(entry, versions);
	}
}
