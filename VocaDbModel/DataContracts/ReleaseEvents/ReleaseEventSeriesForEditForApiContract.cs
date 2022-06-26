using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ReleaseEventSeriesForEditForApiContract
{
	[DataMember]
	public EventCategory Category { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ReleaseEventSeriesForEditForApiContract()
	{
		Description = string.Empty;
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public ReleaseEventSeriesForEditForApiContract(
		ReleaseEventSeries series,
		ContentLanguagePreference languagePreference,
		IAggregatedEntryImageUrlFactory? thumbPersister
	)
	{
		Category = series.Category;
		DefaultNameLanguage = series.TranslatedName.DefaultLanguage;
		Deleted = series.Deleted;
		Description = series.Description;
		Id = series.Id;
		MainPicture = thumbPersister is not null
			? EntryThumbForApiContract.Create(EntryThumb.Create(series), thumbPersister)
			: null;
		Name = series.TranslatedName[languagePreference];
		Names = series.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		Status = series.Status;
		WebLinks = series.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl)
			.ToArray();
	}
}
