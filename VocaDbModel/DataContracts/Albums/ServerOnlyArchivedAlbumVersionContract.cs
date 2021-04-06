#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ServerOnlyArchivedAlbumVersionContract : ServerOnlyArchivedObjectVersionContract
	{
		public ServerOnlyArchivedAlbumVersionContract() { }

		public ServerOnlyArchivedAlbumVersionContract(ArchivedAlbumVersion archivedVersion)
			: base(archivedVersion)
		{
			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : AlbumEditableFields.Nothing);
			Reason = archivedVersion.Reason;
		}

		public AlbumEditableFields ChangedFields { get; init; }

		public AlbumArchiveReason Reason { get; init; }
	}
}
