using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Songs {

	public class SongReport : EntryReport {

		private Song song;

		public SongReport() { }

		public SongReport(Song song, SongReportType reportType, User user, string hostname, string notes) 
			: base(user, hostname, notes) {

			Song = song;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase {
			get { return Song; }
		}

		public virtual SongReportType ReportType { get; set; }

		public virtual Song Song {
			get { return song; }
			set { 
				ParamIs.NotNull(() => value);
				song = value; 
			}
		}

		public override string ToString() {
			return string.Format("Entry report '{0}' for {1} [{2}]", ReportType, EntryBase, Id);
		}

	}

	public enum SongReportType {

		BrokenPV		= 1,

		InvalidInfo		= 2,

		Duplicate		= 3,

		Inappropriate	= 4,

		Other			= 5

	}

}
