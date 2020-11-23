using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	public class ArchivedAlbumVersionContract : ArchivedObjectVersionContract
	{
		public ArchivedAlbumVersionContract() { }

		public ArchivedAlbumVersionContract(ArchivedAlbumVersion archivedVersion)
			: base(archivedVersion)
		{
			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : AlbumEditableFields.Nothing);
			Reason = archivedVersion.Reason;
		}

		public AlbumEditableFields ChangedFields { get; set; }

		public AlbumArchiveReason Reason { get; set; }
	}
}
