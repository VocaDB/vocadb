using System.Collections.Generic;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class SongReport : EntryReport {

        public static readonly HashSet<SongReportType> ReportTypesWithRequiredNotes = 
            new HashSet<SongReportType>{ SongReportType.InvalidInfo, SongReportType.Other };

		private Song song;

		public SongReport() { }

		public SongReport(Song song, SongReportType reportType, User user, string hostname, string notes, int? versionNumber) 
			: base(user, hostname, notes, versionNumber) {

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

		public virtual ArchivedSongVersion Version
		{
			get
			{
				return VersionNumber.HasValue ? Song.ArchivedVersionsManager.GetVersion(VersionNumber.Value) : null;
			}
		}

		public override ArchivedObjectVersion VersionBase
		{
			get { return Version; }
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
