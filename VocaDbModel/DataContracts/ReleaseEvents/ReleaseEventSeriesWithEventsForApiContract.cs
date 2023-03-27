using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ReleaseEventSeriesWithEventsForApiContract
{
	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public ReleaseEventForApiContract[] Events { get; init; }

	[DataMember]
	public int Id { get; set; }

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	public ReleaseEventSeriesWithEventsForApiContract()
	{
		Description = string.Empty;
		Events = Array.Empty<ReleaseEventForApiContract>();
		Name = string.Empty;
	}

	public ReleaseEventSeriesWithEventsForApiContract(
		ReleaseEventSeries series,
		IEnumerable<ReleaseEvent> events,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory? thumbPersister
	)
	{
		Description = series.Description;
		Events = events
			.OrderBy(e => e.SeriesNumber)
			.ThenBy(e => e.Date.DateTime)
			.Select(e => new ReleaseEventForApiContract(
				rel: e,
				languagePreference: languagePreference,
				permissionContext,
				fields: ReleaseEventOptionalFields.None,
				thumbPersister: null
			))
			.ToArray();
		Id = series.Id;
		MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(series), thumbPersister);
		Name = series.TranslatedName[languagePreference];
	}
}
