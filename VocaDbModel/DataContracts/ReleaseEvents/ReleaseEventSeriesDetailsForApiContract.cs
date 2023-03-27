using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ReleaseEventSeriesDetailsForApiContract
{
	[DataMember]
	public string AdditionalNames { get; init; }

	[DataMember]
	public EventCategory Category { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public ReleaseEventForApiContract[] Events { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public TagUsageForApiContract[] Tags { get; init; }

	[DataMember]
	public string UrlSlug { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ReleaseEventSeriesDetailsForApiContract(
		ReleaseEventSeries series,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister
	)
	{
		AdditionalNames = series.Names.GetAdditionalNamesStringForLanguage(languagePreference);
		Category = series.Category;
		Deleted = series.Deleted;
		Description = series.Description;

		Events = series.Events
			.OrderBy(e => e.SeriesNumber)
			.ThenBy(e => e.Date.DateTime)
			.Select(e => new ReleaseEventForApiContract(
				rel: e,
				languagePreference: languagePreference,
				permissionContext,
				fields: ReleaseEventOptionalFields.None,
				thumbPersister: thumbPersister
			))
			.ToArray();

		Id = series.Id;
		MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(series), thumbPersister);
		Name = series.TranslatedName[languagePreference];
		Status = series.Status;

		Tags = series.Tags.ActiveUsages
			.Select(u => new TagUsageForApiContract(tagUsage: u, languagePreference: languagePreference))
			.OrderByDescending(t => t.Count)
			.ToArray();

		UrlSlug = series.UrlSlug;

		WebLinks = series.WebLinks
			.OrderBy(w => w.DescriptionOrUrl)
			.Select(w => new WebLinkForApiContract(webLink: w))
			.ToArray();
	}
}
