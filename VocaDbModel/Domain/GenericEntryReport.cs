using System;
using System.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Domain {

	public abstract class GenericEntryReport<TEntry, TReport> : EntryReport where TEntry : class, IEntryWithNames where TReport: struct, Enum {

		private TEntry song;

		protected GenericEntryReport() { }

		protected GenericEntryReport(TEntry entry, TReport reportType, User user, string hostname, string notes, int? versionNumber) 
			: base(user, hostname, notes, versionNumber) {

			Entry = entry;
			ReportType = reportType;

		}

		public override IEntryWithNames EntryBase => Entry;

		public virtual TReport ReportType { get; set; }

		public virtual TEntry Entry {
			get { return song; }
			set {
				ParamIs.NotNull(() => value);
				song = value;
			}
		}

		public override string ToString() {
			return string.Format("Entry report '{0}' for {1} [{2}]", ReportType, EntryBase, Id);
		}

		public override string TranslatedReportTypeName(IEnumTranslations enumTranslations) {
			return enumTranslations.Translation(ReportType);
		}

		public override string TranslatedReportTypeName(IEnumTranslations enumTranslations, CultureInfo culture) {
			return enumTranslations.Translation(ReportType, culture);
		}

	}

}
