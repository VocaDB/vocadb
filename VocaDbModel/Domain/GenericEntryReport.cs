using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Domain;

public abstract class GenericEntryReport<TEntry, TReport> : EntryReport, IEntryLink<TEntry>
	where TEntry : class, IEntryWithNames
	where TReport : struct, Enum
{
	private TEntry _entry;

#nullable disable
	protected GenericEntryReport() { }
#nullable enable

	protected GenericEntryReport(TEntry entry, TReport reportType, User? user, string hostname, string notes, int? versionNumber)
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
		get => _entry;
		[MemberNotNull(nameof(_entry))]
		set
		{
			ParamIs.NotNull(() => value);
			_entry = value;
		}
	}

	// TODO: Refactor this hacky code
	public override string ReportTypeName()
	{
		var converter = new Newtonsoft.Json.Converters.StringEnumConverter();
		return Newtonsoft.Json.JsonConvert.SerializeObject(ReportType,  converter).Trim('"').Trim('\'');
	}

	public override string ToString()
	{
		return $"Entry report '{ReportType}' for {EntryBase} [{Id}]";
	}

	public override string? TranslatedReportTypeName(IEnumTranslations enumTranslations)
	{
		return enumTranslations.Translation(ReportType);
	}

	public override string? TranslatedReportTypeName(IEnumTranslations enumTranslations, CultureInfo? culture)
	{
		return enumTranslations.Translation(ReportType, culture);
	}
}
