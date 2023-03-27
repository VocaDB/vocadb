using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Discussions;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Api;

[DataContract(Namespace = Schemas.VocaDb)]
public class EntryForApiContract : IEntryWithIntId
{
	public static EntryForApiContract Create(
		IEntryWithNames entry,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
	{
		ParamIs.NotNull(() => entry);

		return entry.EntryType switch
		{
			EntryType.Album => new EntryForApiContract((Album)entry, languagePreference, permissionContext, thumbPersister, includedFields),
			EntryType.Artist => new EntryForApiContract((Artist)entry, languagePreference, permissionContext, thumbPersister, includedFields),
			EntryType.DiscussionTopic => new EntryForApiContract((DiscussionTopic)entry, languagePreference),
			EntryType.ReleaseEvent => new EntryForApiContract((ReleaseEvent)entry, languagePreference, permissionContext, thumbPersister, includedFields),
			EntryType.Song => new EntryForApiContract((Song)entry, languagePreference, includedFields),
			EntryType.SongList => new EntryForApiContract((SongList)entry, permissionContext, thumbPersister, includedFields),
			EntryType.Tag => new EntryForApiContract((Tag)entry, languagePreference, permissionContext, thumbPersister, includedFields),
			_ => new EntryForApiContract(entry, languagePreference, includedFields),
		};
	}

	public EntryForApiContract() { }

	private EntryForApiContract(IEntryWithNames entry, ContentLanguagePreference languagePreference, EntryOptionalFields fields)
	{
		EntryType = entry.EntryType;
		Id = entry.Id;

		DefaultName = entry.DefaultName;
		DefaultNameLanguage = entry.Names.SortNames.DefaultLanguage;
		Name = entry.Names.SortNames[languagePreference];
		Version = entry.Version;

		if (fields.HasFlag(EntryOptionalFields.AdditionalNames))
		{
			AdditionalNames = entry.Names.GetAdditionalNamesStringForLanguage(languagePreference);
		}
	}

	public EntryForApiContract(
		Artist artist,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
		: this(artist, languagePreference, includedFields)
	{
		ActivityDate = artist.ReleaseDate;
		ArtistType = artist.ArtistType;
		CreateDate = artist.CreateDate;
		Status = artist.Status;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && artist.Picture != null)
		{
			MainPicture = EntryThumbForApiContract.Create(new EntryThumb(artist, artist.PictureMime, ImagePurpose.Main), thumbPersister);
		}

		if (includedFields.HasFlag(EntryOptionalFields.Names))
		{
			Names = artist.Names.Select(n => new LocalizedStringContract(n)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.Tags))
		{
			Tags = artist.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.WebLinks))
		{
			WebLinks = artist.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();
		}
	}

	public EntryForApiContract(
		Album album,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
		: this(album, languagePreference, includedFields)
	{
		ActivityDate = album.OriginalReleaseDate.IsFullDate ? (DateTime?)album.OriginalReleaseDate.ToDateTime() : null;
		ArtistString = album.ArtistString[languagePreference];
		CreateDate = album.CreateDate;
		DiscType = album.DiscType;
		Status = album.Status;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && album.CoverPictureData != null)
		{
			MainPicture = new EntryThumbForApiContract(new EntryThumb(album, album.CoverPictureMime, ImagePurpose.Main), thumbPersister);
		}

		if (includedFields.HasFlag(EntryOptionalFields.Names))
		{
			Names = album.Names.Select(n => new LocalizedStringContract(n)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.PVs))
		{
			PVs = album.PVs.Select(p => new PVContract(p)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.Tags))
		{
			Tags = album.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.WebLinks))
		{
			WebLinks = album.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();
		}
	}

	public EntryForApiContract(
		ReleaseEvent releaseEvent,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
		: this(releaseEvent, languagePreference, includedFields)
	{
		ActivityDate = releaseEvent.Date.DateTime;
		EventCategory = releaseEvent.InheritedCategory;
		ReleaseEventSeriesName = releaseEvent.Series?.TranslatedName[languagePreference];
		Status = releaseEvent.Status;
		UrlSlug = releaseEvent.UrlSlug;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture))
		{
			MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(releaseEvent) ?? EntryThumb.Create(releaseEvent.Series), thumbPersister);
		}

		if (includedFields.HasFlag(EntryOptionalFields.WebLinks))
		{
			WebLinks = releaseEvent.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();
		}
	}

	// Only used for recent comments atm.
	public EntryForApiContract(DiscussionTopic topic, ContentLanguagePreference languagePreference)
		: this((IEntryWithNames)topic, languagePreference, EntryOptionalFields.None)
	{
		CreateDate = topic.Created;
	}

	public EntryForApiContract(
		Song song,
		ContentLanguagePreference languagePreference,
		EntryOptionalFields includedFields
	)
		: this((IEntryWithNames)song, languagePreference, includedFields)
	{
		ActivityDate = song.PublishDate.DateTime;
		ArtistString = song.ArtistString[languagePreference];
		CreateDate = song.CreateDate;
		SongType = song.SongType;
		Status = song.Status;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture))
		{
			var thumb = song.GetThumbUrl();

			if (!string.IsNullOrEmpty(thumb))
			{
				MainPicture = new EntryThumbForApiContract { UrlSmallThumb = thumb, UrlThumb = thumb, UrlTinyThumb = thumb };
			}
		}

		if (includedFields.HasFlag(EntryOptionalFields.Names))
		{
			Names = song.Names.Select(n => new LocalizedStringContract(n)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.PVs))
		{
			PVs = song.PVs.Select(p => new PVContract(p)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.Tags))
		{
			Tags = song.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, languagePreference)).ToArray();
		}

		if (includedFields.HasFlag(EntryOptionalFields.WebLinks))
		{
			WebLinks = song.WebLinks.Select(w => new ArchivedWebLinkContract(w)).ToArray();
		}
	}

	public EntryForApiContract(
		SongList songList,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
		: this(songList, ContentLanguagePreference.Default, includedFields)
	{
		ActivityDate = songList.EventDate;
		CreateDate = songList.CreateDate;
		SongListFeaturedCategory = songList.FeaturedCategory;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && songList.Thumb != null)
		{
			MainPicture = new EntryThumbForApiContract(songList.Thumb, thumbPersister, SongList.ImageSizes);
		}
	}

	public EntryForApiContract(
		Tag tag,
		ContentLanguagePreference languagePreference,
		IUserPermissionContext permissionContext,
		IAggregatedEntryImageUrlFactory thumbPersister,
		EntryOptionalFields includedFields
	)
		: this(tag, languagePreference, includedFields)
	{
		CreateDate = tag.CreateDate;
		Status = tag.Status;
		TagCategoryName = tag.CategoryName;

		if (includedFields.HasFlag(EntryOptionalFields.MainPicture) && tag.Thumb != null)
		{
			MainPicture = new EntryThumbForApiContract(tag.Thumb, thumbPersister, Tag.ImageSizes);
		}

		if (includedFields.HasFlag(EntryOptionalFields.WebLinks))
		{
			WebLinks = tag.WebLinks.Links.Select(w => new ArchivedWebLinkContract(w)).ToArray();
		}

		UrlSlug = tag.UrlSlug;
	}

	/// <summary>
	/// Date when this entry was published or the indicated activity happened.
	/// Depends on entry type.
	/// </summary>
	/// <remarks>
	/// For albums and songs: publish date.
	/// For events and song lists: event date.
	/// </remarks>
	[DataMember(EmitDefaultValue = false)]
	public DateTime? ActivityDate { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? AdditionalNames { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? ArtistString { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public ArtistType? ArtistType { get; init; }

	[DataMember]
	public DateTime CreateDate { get; init; }

	[DataMember]
	public string DefaultName { get; init; } = string.Empty;

	[DataMember]
	public ContentLanguageSelection DefaultNameLanguage { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? Description { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public DiscType? DiscType { get; init; }

	[DataMember]
	public EntryType EntryType { get; init; }

	/// <summary>
	/// Event category.
	/// Inherited from event series, if the event has series.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public EventCategory? EventCategory { get; init; }

	[DataMember]
	public int Id { get; set; }

	/// <summary>
	/// Entry main picture, in multiple sizes if available.
	/// For songs this is the thumbnail.
	/// </summary>
	[DataMember(EmitDefaultValue = false)]
	public EntryThumbForApiContract? MainPicture { get; init; }

	[DataMember]
	public string Name { get; init; } = string.Empty;

	[DataMember(EmitDefaultValue = false)]
	public LocalizedStringContract[]? Names { get; init; }

	/// <summary>
	/// List of PVs, for songs and albums. Optional field.
	/// </summary>
	[DataMember(EmitDefaultValue = false, Name = "pvs")]
	public PVContract[]? PVs { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public SongListFeaturedCategory? SongListFeaturedCategory { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public SongType? SongType { get; init; }

	[DataMember]
	public EntryStatus Status { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? ReleaseEventSeriesName { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? TagCategoryName { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public TagUsageForApiContract[]? Tags { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public string? UrlSlug { get; init; }

	[DataMember]
	public int Version { get; init; }

	[DataMember(EmitDefaultValue = false)]
	public ArchivedWebLinkContract[]? WebLinks { get; init; }
}

[Flags]
public enum EntryOptionalFields
{
	None = 0,
	AdditionalNames = 1 << 0,
	Description = 1 << 1,
	MainPicture = 1 << 2,
	Names = 1 << 3,

	/// <summary>
	/// List of PVs, for songs and albums
	/// </summary>
	PVs = 1 << 4,

	Tags = 1 << 5,
	WebLinks = 1 << 6,
}
