using System;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain
{
	public interface IEntryWithVersions : IEntryBase
	{
		IArchivedVersionsManager ArchivedVersionsManager { get; }
	}

	public interface IEntryWithVersions<TVersion, TField> : IEntryWithVersions
		where TVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<TField>
		where TField : struct, IConvertible
	{
		new ArchivedVersionManager<TVersion, TField> ArchivedVersionsManager { get; }
	}
}
