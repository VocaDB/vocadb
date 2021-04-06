#nullable disable

using System.Runtime.Serialization;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ServerOnlyArchivedSongListVersionContract : ServerOnlyArchivedObjectVersionContract
	{
		public ServerOnlyArchivedSongListVersionContract() { }

		public ServerOnlyArchivedSongListVersionContract(ArchivedSongListVersion archivedVersion)
			: base(archivedVersion)
		{
			ChangedFields = archivedVersion.Diff.ChangedFields.Value;
			Reason = archivedVersion.CommonEditEvent;
		}

		public SongListEditableFields ChangedFields { get; init; }

		public EntryEditEvent Reason { get; init; }
	}
}
