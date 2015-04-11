using System.Collections.Generic;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums {

	public class AlbumReport : EntryReport {

        public static readonly HashSet<AlbumReportType> ReportTypesWithRequiredNotes = 
            new HashSet<AlbumReportType>{ AlbumReportType.InvalidInfo, AlbumReportType.Other };

		private Album album;

		public AlbumReport() { }

		public AlbumReport(Album album, AlbumReportType reportType, User user, string hostname, string notes, int? versionNumber)
			: base(user, hostname, notes, versionNumber) {

			Album = album;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase {
			get { return Album; }
		}

		public virtual AlbumReportType ReportType { get; set; }

		public virtual Album Album {
			get { return album; }
			set {
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual ArchivedAlbumVersion Version {
			get {
				return VersionNumber.HasValue ? Album.ArchivedVersionsManager.GetVersion(VersionNumber.Value) : null;
			}
		}

		public override ArchivedObjectVersion VersionBase {
			get { return Version; }
		}

		public override string ToString() {
			return string.Format("Entry report '{0}' for {1} [{2}]", ReportType, EntryBase, Id);
		}

	}

	public enum AlbumReportType {

		InvalidInfo = 1,

		Duplicate = 2,

		Inappropriate = 3,

		Other = 4

	}

}
