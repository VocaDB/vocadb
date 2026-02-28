using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts.ReleaseEvents;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record ReleaseEventForEditForApiContract
{
	[DataMember]
	public ReleaseEventSeriesForApiContract[] AllSeries { get; init; }

	[DataMember]
	public ArtistForEventContract[] Artists { get; init; }

	[DataMember]
	public EventCategory Category { get; init; }

	[DataMember]
	public bool CustomName { get; set; }

	[DataMember]
	public DateTime? Date { get; init; }

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; set; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public string Description { get; init; }

	[DataMember]
	public DateTime? EndDate { get; init; }

	[DataMember]
	public int Id { get; init; }

	/// <summary>
	/// Main picture.
	/// This IS inherited from series.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public LocalizedStringWithIdContract[] Names { get; init; }

	[DataMember(Name = "pvs")]
	public PVContract[] PVs { get; init; }

	[DataMember]
	public ReleaseEventSeriesForApiContract? Series { get; set; }

	[DataMember]
	public int SeriesNumber { get; init; }

	[DataMember]
	public string SeriesSuffix { get; set; }

	[DataMember]
	public SongListBaseContract? SongList { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public VenueForApiContract? Venue { get; init; }

	[DataMember]
	public string? VenueName { get; init; }

	[DataMember]
	public string UpdateNotes { get; set; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public ReleaseEventForEditForApiContract()
	{
		AllSeries = Array.Empty<ReleaseEventSeriesForApiContract>();
		Artists = Array.Empty<ArtistForEventContract>();
		Description = string.Empty;
		Name = string.Empty;
		Names = Array.Empty<LocalizedStringWithIdContract>();
		PVs = Array.Empty<PVContract>();
		SeriesSuffix = string.Empty;
		UpdateNotes = string.Empty;
		WebLinks = Array.Empty<WebLinkForApiContract>();
	}

	public ReleaseEventForEditForApiContract(
		ReleaseEvent releaseEvent,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		ReleaseEventSeriesForApiContract[] allSeries,
		IAggregatedEntryImageUrlFactory thumbPersister
	)
	{
		AllSeries = allSeries;
		Artists = releaseEvent.AllArtists
			.Select(a => new ArtistForEventContract(a, languagePreference))
			.OrderBy(a => a.Artist is not null ? a.Artist.Name : a.Name)
			.ToArray();
		Category = releaseEvent.Category;
		CustomName = releaseEvent.CustomName;
		Date = releaseEvent.Date;
		DefaultNameLanguage = releaseEvent.TranslatedName.DefaultLanguage;
		Deleted = releaseEvent.Deleted;
		Description = releaseEvent.Description;
		EndDate = releaseEvent.EndDate;
		Id = releaseEvent.Id;
		MainPicture = permissionContext.HasPermission(PermissionToken.ViewCoverArtImages)
			? EntryThumbForApiContract.Create(EntryThumb.Create(releaseEvent) ?? EntryThumb.Create(releaseEvent.Series), thumbPersister)
			: null;
		Name = releaseEvent.TranslatedName[languagePreference];
		Names = releaseEvent.Names
			.Select(n => new LocalizedStringWithIdContract(n))
			.ToArray();
		PVs = releaseEvent.PVs.Select(p => new PVContract(p)).ToArray();
		Series = releaseEvent.HasSeries
			? new ReleaseEventSeriesForApiContract(
				series: releaseEvent.Series,
				languagePreference: languagePreference,
				permissionContext,
				fields: ReleaseEventSeriesOptionalFields.None,
				thumbPersister: null
			)
			: null;
		SeriesNumber = releaseEvent.SeriesNumber;
		SeriesSuffix = releaseEvent.SeriesSuffix;
		SongList = ObjectHelper.Convert(releaseEvent.SongList, s => new SongListBaseContract(s));
		Status = releaseEvent.Status;
		Venue = ObjectHelper.Convert(releaseEvent.Venue, v => new VenueForApiContract(v, languagePreference, fields: VenueOptionalFields.None));
		VenueName = releaseEvent.VenueName;
		UpdateNotes = string.Empty;
		WebLinks = releaseEvent.WebLinks
			.Select(w => new WebLinkForApiContract(w))
			.OrderBy(w => w.DescriptionOrUrl).
			ToArray();
	}
}
