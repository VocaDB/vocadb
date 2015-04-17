using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain {

	public interface IEntryWithVersions : IEntryBase {

		IArchivedVersionsManager ArchivedVersionsManager { get; }

	}

}
