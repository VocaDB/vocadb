#nullable disable

using System;
using System.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Domain
{
	public abstract class GenericEntryReport<TEntry, TReport> : EntryReport, IEntryLink<TEntry>
		where TEntry : class, IEntryWithNames
		where TReport : struct, Enum
	{
		private TEntry _song;

		protected GenericEntryReport() { }

		protected GenericEntryReport(TEntry entry, TReport reportType, User user, string hostname, string notes, int? versionNumber)
			: base(user, hostname, notes, versionNumber)
		{
			Entry = entry;
			ReportType = reportType;
		}

		public override IEntryWithNames EntryBase => Entry;

		public virtual TReport ReportType { get; set; }

		/// <summary>
		/// Reported entry. Cannot be null.
		/// This field is mapped to database, but different column depending on linked entry type.
		/// </summary>
		public virtual TEntry Entry
		{
			get => _song;
			set
			{
				ParamIs.NotNull(() => value);
				_song = value;
			}
		}

#nullable enable
		public override string ToString()
		{
			return $"Entry report '{ReportType}' for {EntryBase} [{Id}]";
		}
#nullable disable

		public override string TranslatedReportTypeName(IEnumTranslations enumTranslations)
		{
			return enumTranslations.Translation(ReportType);
		}

		public override string TranslatedReportTypeName(IEnumTranslations enumTranslations, CultureInfo culture)
		{
			return enumTranslations.Translation(ReportType, culture);
		}
	}
}
