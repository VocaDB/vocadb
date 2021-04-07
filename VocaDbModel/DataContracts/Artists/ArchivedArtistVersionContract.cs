#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Artists
{
	public class ArchivedArtistVersionContract : ArchivedObjectVersionContract
	{
		public ArchivedArtistVersionContract() { }

		public ArchivedArtistVersionContract(ArchivedArtistVersion archivedVersion, IUserIconFactory userIconFactory)
			: base(archivedVersion, userIconFactory)
		{
			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : ArtistEditableFields.Nothing);
			Reason = archivedVersion.Reason;
		}

		public ArtistEditableFields ChangedFields { get; init; }

		public ArtistArchiveReason Reason { get; init; }
	}
}
