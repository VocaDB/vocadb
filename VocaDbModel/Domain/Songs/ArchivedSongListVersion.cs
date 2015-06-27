using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class ArchivedSongListVersion : ArchivedObjectVersion, IArchivedObjectVersionWithFields<SongListEditableFields> {

		private SongListDiff diff;
		private SongList songList;

		public ArchivedSongListVersion() { 
			Status = EntryStatus.Finished;
		}

		public ArchivedSongListVersion(SongList songList, SongListDiff diff, AgentLoginData author,
			EntryStatus status,
			EntryEditEvent commonEditEvent, string notes)
			: base(null, author, songList.Version, status, notes) {

			ParamIs.NotNull(() => diff);

			SongList = songList;
			Diff = diff;
			CommonEditEvent = commonEditEvent;

		}

		public virtual EntryEditEvent CommonEditEvent { get; set; }

		public override IEntryDiff DiffBase {
			get { return Diff; }
		}

		public virtual SongListDiff Diff {
			get { return diff; }
			set { 
				ParamIs.NotNull(() => value);
				diff = value; 
			}
		}

		public override EntryEditEvent EditEvent {
			get { return CommonEditEvent; }
		}

		public override IEntryWithNames EntryBase {
			get { return SongList; }
		}

		public virtual SongList SongList {
			get { return songList; }
			set { 
				ParamIs.NotNull(() => value);
				songList = value; 
			}
		}

		public virtual bool IsIncluded(SongListEditableFields field) {
			return true;
		}

	}

}
