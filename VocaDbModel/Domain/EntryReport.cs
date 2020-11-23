using System;
using System.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Versioning;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Domain
{

	public abstract class EntryReport : IEntryWithIntId
	{

		public const int MaxNotesLength = 400;

		private string hostname;
		private string notes;

		protected EntryReport()
		{
			Created = DateTime.UtcNow;
			Notes = string.Empty;
			Status = ReportStatus.Open;
		}

		protected EntryReport(User user, string hostname, string notes, int? versionNumber)
			: this()
		{

			User = user;
			Hostname = hostname;
			Notes = notes;
			VersionNumber = versionNumber;

		}

		public virtual DateTime? ClosedAt { get; set; }

		public virtual User ClosedBy { get; set; }

		public virtual DateTime Created { get; set; }

		public abstract IEntryWithNames EntryBase { get; }

		public virtual EntryType EntryType => EntryBase.EntryType;

		/// <summary>
		/// Hostname/IP address of the user who created the report. This can be null or empty.
		/// </summary>
		public virtual string Hostname
		{
			get => hostname;
			set => hostname = value;
		}

		public virtual int Id { get; set; }

		/// <summary>
		/// Report notes. Cannot be null, but can be empty.
		/// </summary>
		public virtual string Notes
		{
			get => notes;
			set
			{
				ParamIs.NotNull(() => value);
				notes = value;
			}
		}

		public virtual ReportStatus Status { get; set; }

		public abstract string TranslatedReportTypeName(IEnumTranslations enumTranslations);

		public abstract string TranslatedReportTypeName(IEnumTranslations enumTranslations, CultureInfo culture);

		/// <summary>
		/// User who created the report. This can be null if the report was created by the system.
		/// </summary>
		public virtual User User { get; set; }

		public virtual ArchivedObjectVersion VersionBase => null;

		public virtual int? VersionNumber { get; set; }

	}

	public enum ReportStatus
	{
		Open,
		Closed
	}

}
