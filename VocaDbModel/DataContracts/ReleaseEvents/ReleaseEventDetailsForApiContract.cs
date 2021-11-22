using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public sealed record ReleaseEventDetailsForApiContract
	{
		[DataMember]
		public string AdditionalNames { get; init; }

		[DataMember]
		public AlbumForApiContract[] Albums { get; init; }

		[DataMember]
		public ArtistForEventContract[] Artists { get; init; }

		[DataMember]
		public bool CanRemoveTagUsages { get; init; }

		[DataMember]
		public EventCategory Category { get; init; }

		[DataMember]
		public DateTime? Date { get; init; }

		[DataMember]
		public bool Deleted { get; init; }

		[DataMember]
		public string Description { get; init; }

		[DataMember]
		public DateTime? EndDate { get; init; }

		[DataMember]
		public UserEventRelationshipType? EventAssociationType { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public EventCategory InheritedCategory { get; init; }

		[DataMember]
		public TagBaseContract? InheritedCategoryTag { get; init; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; init; }

		/// <summary>
		/// Main picture.
		/// This IS inherited from series.
		/// </summary>
		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract? MainPicture { get; init; }

		[DataMember]
		public string Name { get; init; }

		[DataMember]
		[JsonProperty("pvs")]
		public PVContract[] PVs { get; init; }

		[DataMember]
		public ReleaseEventSeriesForApiContract? Series { get; init; }

		[DataMember]
		public SongListBaseContract? SongList { get; init; }

		[DataMember]
		public SongInListForApiContract[]? SongListSongs { get; init; }

		[DataMember]
		public SongForApiContract[] Songs { get; init; }

		[DataMember]
		public EntryStatus Status { get; init; }

		[DataMember]
		public TagUsageForApiContract[] Tags { get; init; }

		[DataMember]
		public string UrlSlug { get; init; }

		[DataMember]
		public UserForApiContract[] UsersAttending { get; init; }

		[DataMember]
		public VenueForApiContract? Venue { get; init; }

		[DataMember]
		public string? VenueName { get; init; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; init; }

		public ReleaseEventDetailsForApiContract(
			ReleaseEvent releaseEvent,
			ContentLanguagePreference languagePreference,
			IUserPermissionContext userContext,
			IUserIconFactory userIconFactory,
			IEntryTypeTagRepository? entryTypeTags,
			UserEventRelationshipType? eventAssociationType,
			CommentForApiContract[] latestComments,
			IAggregatedEntryImageUrlFactory thumbPersister
		)
		{
			AdditionalNames = releaseEvent.Names.GetAdditionalNamesStringForLanguage(languagePreference);

			Albums = releaseEvent.Albums
				.Select(a => new AlbumForApiContract(
					album: a,
					languagePreference: languagePreference,
					thumbPersister: thumbPersister,
					fields: AlbumOptionalFields.AdditionalNames | AlbumOptionalFields.MainPicture
				))
				.OrderBy(a => a.Name)
				.ToArray();

			Artists = releaseEvent.AllArtists
				.Select(a => new ArtistForEventContract(artistForEvent: a, languagePreference: languagePreference))
				.OrderBy(a => a.Artist is not null ? a.Artist.Name : a.Name)
				.ToArray();

			CanRemoveTagUsages = EntryPermissionManager.CanRemoveTagUsages(userContext, releaseEvent);
			Category = releaseEvent.Category;
			Date = releaseEvent.Date;
			Deleted = releaseEvent.Deleted;
			Description = releaseEvent.Description;
			EndDate = releaseEvent.EndDate;
			EventAssociationType = eventAssociationType;
			Id = releaseEvent.Id;

			LatestComments = latestComments;
			MainPicture = EntryThumbForApiContract.Create(EntryThumb.Create(releaseEvent) ?? EntryThumb.Create(releaseEvent.Series), thumbPersister);
			Name = releaseEvent.TranslatedName[languagePreference];
			PVs = releaseEvent.PVs.Select(p => new PVContract(pv: p)).ToArray();

			Series = releaseEvent.Series is ReleaseEventSeries series
				? new ReleaseEventSeriesForApiContract(
					series: series,
					languagePreference: languagePreference,
					fields:
						ReleaseEventSeriesOptionalFields.AdditionalNames |
						ReleaseEventSeriesOptionalFields.Description |
						ReleaseEventSeriesOptionalFields.MainPicture |
						ReleaseEventSeriesOptionalFields.WebLinks,
					thumbPersister: thumbPersister
				)
				: null;

			InheritedCategory = Series?.Category ?? Category;

			var categoryTag = entryTypeTags?.GetTag(EntryType.ReleaseEvent, InheritedCategory);
			InheritedCategoryTag = categoryTag is not null ? new TagBaseContract(tag: categoryTag, languagePreference: languagePreference) : null;

			SongList = ObjectHelper.Convert(releaseEvent.SongList, s => new SongListBaseContract(s));

			SongListSongs = releaseEvent.SongList is SongList songList
				? songList.SongLinks
					.OrderBy(s => s.Order)
					.Select(s => new SongInListForApiContract(
						songInList: s,
						languagePreference: languagePreference,
						fields: SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl
					))
					.ToArray()
				: null;

			Songs = releaseEvent.Songs
				.Select(s => new SongForApiContract(
					song: s,
					languagePreference: languagePreference,
					fields: SongOptionalFields.AdditionalNames | SongOptionalFields.ThumbUrl
				))
				.OrderBy(s => s.Name)
				.ToArray();

			Status = releaseEvent.Status;

			Tags = releaseEvent.Tags.ActiveUsages
				.Select(u => new TagUsageForApiContract(tagUsage: u, languagePreference: languagePreference))
				.OrderByDescending(t => t.Count)
				.ToArray();

			UrlSlug = releaseEvent.UrlSlug;

			UsersAttending = releaseEvent.Users
				.Where(u => u.RelationshipType == UserEventRelationshipType.Attending)
				.Select(u => new UserForApiContract(
					user: u.User,
					iconFactory: userIconFactory,
					optionalFields: UserOptionalFields.MainPicture
				))
				.ToArray();

			Venue = ObjectHelper.Convert(releaseEvent.Venue, v => new VenueForApiContract(
				venue: v,
				languagePreference: languagePreference,
				fields: VenueOptionalFields.AdditionalNames | VenueOptionalFields.Description | VenueOptionalFields.WebLinks
			));

			VenueName = releaseEvent.VenueName;

			WebLinks = releaseEvent.WebLinks
				.Select(w => new WebLinkForApiContract(webLink: w))
				.OrderBy(w => w.DescriptionOrUrl)
				.ToArray();
		}
	}
}
