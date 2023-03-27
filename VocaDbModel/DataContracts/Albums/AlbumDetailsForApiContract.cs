using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Albums;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record AlbumDetailsForApiContract
{
	[DataMember]
	public string AdditionalNames { get; init; }

	[DataMember]
	public AlbumForUserForApiContract? AlbumForUser { get; set; }

	[DataMember]
	public ArtistForAlbumForApiContract[] ArtistLinks { get; init; }

	[DataMember]
	public string ArtistString { get; init; }

	[DataMember]
	public bool CanEditPersonalDescription { get; init; }

	[DataMember]
	public bool CanRemoveTagUsages { get; init; }

	[DataMember]
	public int CommentCount { get; init; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public bool Deleted { get; init; }

	[DataMember]
	public EnglishTranslatedStringContract Description { get; init; }

	[DataMember]
	public Dictionary<int, AlbumDiscPropertiesContract> Discs { get; init; }

	[DataMember]
	[JsonConverter(typeof(StringEnumConverter))]
	public DiscType DiscType { get; init; }

	[DataMember]
	public TagBaseContract? DiscTypeTag { get; init; }

	[DataMember]
	public int Hits { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public CommentForApiContract[] LatestComments { get; set; } = default!;

	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public AlbumForApiContract? MergedTo { get; set; }

	[DataMember]
	public string Name { get; init; }

	[DataMember]
	public AlbumReleaseContract? OriginalRelease { get; init; }

	[DataMember]
	public ArtistForApiContract? PersonalDescriptionAuthor { get; init; }

	[DataMember]
	public string? PersonalDescriptionText { get; init; }

	[DataMember]
	public EntryThumbForApiContract[] Pictures { get; init; }

	[DataMember]
	[JsonProperty("pvs")]
	public PVContract[] PVs { get; init; }

	[DataMember]
	public double RatingAverage { get; init; }

	[DataMember]
	public int RatingCount { get; init; }

	[DataMember]
	public SongInAlbumForApiContract[] Songs { get; init; }

	[DataMember]
	public SharedAlbumStatsContract Stats { get; init; } = default!;

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember]
	public TagUsageForApiContract[] Tags { get; init; }

	[DataMember]
	public int TotalLengthSeconds { get; init; }

	[DataMember]
	public int Version { get; init; }

	[DataMember]
	public WebLinkForApiContract[] WebLinks { get; init; }

	public AlbumDetailsForApiContract(
		Album album,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext userContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		Func<Song, SongVoteRating?>? getSongRating = null,
		Tag? discTypeTag = null
	)
	{
		AdditionalNames = album.Names.GetAdditionalNamesStringForLanguage(languagePreference);

		ArtistLinks = album.Artists
			.Select(a => new ArtistForAlbumForApiContract(a, languagePreference))
			.OrderBy(a => a.Name)
			.ToArray();

		ArtistString = album.ArtistString.GetBestMatch(languagePreference);
		CanEditPersonalDescription = EntryPermissionManager.CanEditPersonalDescription(userContext, album);
		CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, album);
		CreateDate = album.CreateDate;
		Deleted = album.Deleted;
		Description = new EnglishTranslatedStringContract(album.Description);

		Discs = album.Songs.Any(s => s.DiscNumber > 1)
			? album.Discs.Select(d => new AlbumDiscPropertiesContract(d)).ToDictionary(a => a.DiscNumber)
			: new Dictionary<int, AlbumDiscPropertiesContract>(0);

		DiscType = album.DiscType;

		DiscTypeTag = discTypeTag is not null
			? new TagBaseContract(discTypeTag, languagePreference)
			: null;

		Id = album.Id;

		MainPicture = album.Thumb is not null
			? new EntryThumbForApiContract(image: album.Thumb, thumbPersister: thumbPersister)
			: null;

		Name = album.TranslatedName[languagePreference];

		OriginalRelease = album.OriginalRelease is not null
			? new AlbumReleaseContract(
				release: album.OriginalRelease,
				languagePreference: languagePreference,
				userContext
			)
			: null;

		PersonalDescriptionAuthor = album.PersonalDescriptionAuthor is { } author
			? new ArtistForApiContract(
				artist: author,
				languagePreference: languagePreference,
				userContext,
				thumbPersister: thumbPersister,
				includedFields: ArtistOptionalFields.MainPicture
			)
			: null;

		PersonalDescriptionText = album.PersonalDescriptionText;
		Pictures = album.Pictures.Select(p => new EntryThumbForApiContract(image: p, thumbPersister: thumbPersister, name: p.Name)).ToArray();
		PVs = album.PVs.Select(p => new PVContract(pv: p)).ToArray();
		RatingAverage = album.RatingAverage;
		RatingCount = album.RatingCount;

		Songs = album.Songs
			.OrderBy(s => s.DiscNumber)
			.ThenBy(s => s.TrackNumber)
			.Select(s => new SongInAlbumForApiContract(
				songInAlbum: s,
				languagePreference: languagePreference,
				userContext,
				fields: SongOptionalFields.None,
				rating: getSongRating?.Invoke(s.Song)
			))
			.ToArray();

		Status = album.Status;

		Tags = album.Tags.ActiveUsages
			.Select(u => new TagUsageForApiContract(u, languagePreference))
			.OrderByDescending(t => t.Count)
			.ToArray();

		TotalLengthSeconds = Songs.All(s => s.Song is not null && s.Song.LengthSeconds > 0)
			? Songs.Sum(s => s.Song.LengthSeconds)
			: 0;

		Version = album.Version;

		WebLinks = album.WebLinks
			.OrderBy(w => w.DescriptionOrUrl)
			.Select(w => new WebLinkForApiContract(webLink: w))
			.ToArray();
	}
}
