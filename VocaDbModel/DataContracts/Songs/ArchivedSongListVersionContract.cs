#nullable disable

using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	public class ArchivedSongListVersionContract : ArchivedObjectVersionContract
	{
		public ArchivedSongListVersionContract() { }

		public ArchivedSongListVersionContract(ArchivedSongListVersion archivedVersion)
			: base(archivedVersion)
		{
			ChangedFields = archivedVersion.Diff.ChangedFields.Value;
			Reason = archivedVersion.CommonEditEvent;
		}

		public SongListEditableFields ChangedFields { get; init; }

		public EntryEditEvent Reason { get; init; }
	}
}
