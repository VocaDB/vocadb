using System;
using System.Collections.Generic;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.DataContracts.UseCases
{
	public class EntryWithArchivedVersionsContract<TEntry, TVersion> : IEntryWithArchivedVersionsContract<TEntry, TVersion>
	{
		public EntryWithArchivedVersionsContract() { }

		public EntryWithArchivedVersionsContract(TEntry entry, TVersion[] archivedVersions)
		{
			ArchivedVersions = archivedVersions;
			Entry = entry;
		}

		public TVersion[] ArchivedVersions { get; set; }

		public TEntry Entry { get; set; }
	}

	public interface IEntryWithArchivedVersionsContract<TEntry, out TVersion>
	{
		TVersion[] ArchivedVersions { get; }

		TEntry Entry { get; }
	}

	public static class EntryWithArchivedVersionsContract
	{
		public static EntryWithArchivedVersionsContract<TEntry, TVersion> Create<TEntry, TVersion>(TEntry entry, TVersion[] versions)
		{
			return new EntryWithArchivedVersionsContract<TEntry, TVersion>(entry, versions);
		}
	}
}
