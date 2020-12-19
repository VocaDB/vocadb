#nullable disable

using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs
{
	public class ArchivedSongListVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<SongListEditableFields>
	{
		private SongListDiff _diff;
		private SongList _songList;

		public ArchivedSongListVersion()
		{
			Status = EntryStatus.Finished;
		}

		public ArchivedSongListVersion(SongList songList, SongListDiff diff, AgentLoginData author,
			EntryStatus status,
			EntryEditEvent commonEditEvent, string notes)
			: base(null, author, songList.Version, status, notes)
		{
			ParamIs.NotNull(() => diff);

			SongList = songList;
			Diff = diff;
			CommonEditEvent = commonEditEvent;
		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase => Diff;

		public virtual SongListDiff Diff
		{
			get => _diff;
			set
			{
				ParamIs.NotNull(() => value);
				_diff = value;
			}
		}

		public override EntryEditEvent EditEvent => CommonEditEvent;

		public override IEntryWithNames EntryBase => SongList;

		public virtual SongList SongList
		{
			get => _songList;
			set
			{
				ParamIs.NotNull(() => value);
				_songList = value;
			}
		}

		public virtual bool IsIncluded(SongListEditableFields field)
		{
			return true;
		}
	}
}
