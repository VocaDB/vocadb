using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums;

[Obsolete]
public class ArchivedAlbumVersionContract : ArchivedObjectVersionContract
{
	public ArchivedAlbumVersionContract() { }

	public ArchivedAlbumVersionContract(ArchivedAlbumVersion archivedVersion, IUserIconFactory userIconFactory)
		: base(archivedVersion, userIconFactory)
	{
		ChangedFields = archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : AlbumEditableFields.Nothing;
		Reason = archivedVersion.Reason;
	}

	public AlbumEditableFields ChangedFields { get; init; }

	public AlbumArchiveReason Reason { get; init; }
}
