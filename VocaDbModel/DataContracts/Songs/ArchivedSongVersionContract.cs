#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ArchivedSongVersionContract : ArchivedObjectVersionContract
	{
		public ArchivedSongVersionContract() { }

		public ArchivedSongVersionContract(ArchivedSongVersion archivedVersion, IUserIconFactory userIconFactory)
			: base(archivedVersion, userIconFactory)
		{
			ChangedFields = (archivedVersion.Diff != null ? archivedVersion.Diff.ChangedFields.Value : SongEditableFields.Nothing);
			Reason = archivedVersion.Reason;
		}

		public SongEditableFields ChangedFields { get; init; }

		public SongArchiveReason Reason { get; init; }
	}
}
