using System.Collections.Generic;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistReport : EntryReport {

        public static readonly HashSet<ArtistReportType> ReportTypesWithRequiredNotes = 
            new HashSet<ArtistReportType>{ ArtistReportType.InvalidInfo, ArtistReportType.Other };

		private Artist artist;

		public ArtistReport() { }

		public ArtistReport(Artist artist, ArtistReportType reportType, User user, string hostname, string notes, int? versionNumber)
			: base(user, hostname, notes, versionNumber) {

			Artist = artist;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase {
			get { return Artist; }
		}

		public virtual ArtistReportType ReportType { get; set; }

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public override string ToString() {
			return string.Format("Entry report '{0}' for {1} [{2}]", ReportType, EntryBase, Id);
		}

	}

	public enum ArtistReportType {

		InvalidInfo = 1,

		Duplicate = 2,

		Inappropriate = 3,

		OwnershipClaim = 4,

		Other = 5

	}

}
