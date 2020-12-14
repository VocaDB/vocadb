#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists
{
	public class ArchivedArtistVersionContract : ArchivedObjectVersionContract
	{
		public ArchivedArtistVersionContract() { }

		public ArchivedArtistVersionContract(ArchivedArtistVersion archivedVersion)
			: base(archivedVersion)
		{
			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : ArtistEditableFields.Nothing);
			Reason = archivedVersion.Reason;
		}

		public ArtistEditableFields ChangedFields { get; set; }

		public ArtistArchiveReason Reason { get; set; }
	}
}
