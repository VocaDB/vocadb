using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Queries;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Events;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries {

	public class EventQueries : QueriesBase<IEventRepository, ReleaseEvent> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEnumTranslations enumTranslations;
		private readonly IFollowedArtistNotifier followedArtistNotifier;
		private readonly IAggregatedEntryImageUrlFactory imageUrlFactory;
		private readonly IEntryThumbPersister imagePersister;
		private readonly IUserMessageMailer mailer;
		private readonly IUserIconFactory userIconFactory;

		private ArchivedReleaseEventVersion Archive(IDatabaseContext<ReleaseEvent> ctx, ReleaseEvent releaseEvent, ReleaseEventDiff diff, EntryEditEvent reason, string notes) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = ArchivedReleaseEventVersion.Create(releaseEvent, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		private ArchivedReleaseEventSeriesVersion Archive(IDatabaseContext<ReleaseEvent> ctx, ReleaseEventSeries releaseEvent, ReleaseEventSeriesDiff diff, EntryEditEvent reason, string notes) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = ArchivedReleaseEventSeriesVersion.Create(releaseEvent, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		public EventQueries(IEventRepository eventRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext,
			IEntryThumbPersister imagePersister, IUserIconFactory userIconFactory, IEnumTranslations enumTranslations, 
			IUserMessageMailer mailer, IFollowedArtistNotifier followedArtistNotifier, IAggregatedEntryImageUrlFactory imageUrlFactory)
			: base(eventRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;
			this.enumTranslations = enumTranslations;
			this.mailer = mailer;
			this.followedArtistNotifier = followedArtistNotifier;
			this.imageUrlFactory = imageUrlFactory;

		}

		public (bool created, int reportId) CreateReport(int eventId, EventReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory,
					(song, reporter, notesTruncated) => new EventReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != EventReportType.Other ? enumTranslations.Translation(reportType) : null,
					eventId, reportType, hostname, notes);
			});

		}

		public void Delete(int eventId, string notes) {

			permissionContext.VerifyManageDatabase();

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(eventId);

				PermissionContext.VerifyEntryDelete(entry);

				entry.Deleted = true;
				ctx.Update(entry);

				Archive(ctx, entry, new ReleaseEventDiff(false), EntryEditEvent.Deleted, notes);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}

		public void DeleteSeries(int id, string notes) {

			permissionContext.VerifyManageDatabase();

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load<ReleaseEventSeries>(id);

				PermissionContext.VerifyEntryDelete(entry);

				entry.Deleted = true;
				ctx.Update(entry);

				Archive(ctx, entry, new ReleaseEventSeriesDiff(false), EntryEditEvent.Deleted, notes);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}

		public PartialFindResult<TResult> Find<TResult>(Func<ReleaseEvent, TResult> fac, EventQueryParams queryParams) {

			return HandleQuery(ctx => {

				var q = ctx.Query()
					.WhereNotDeleted()
					.WhereHasArtists(queryParams.ArtistIds, queryParams.ChildVoicebanks, queryParams.IncludeMembers)
					.WhereHasCategory(queryParams.Category)
					.WhereHasName(queryParams.TextQuery)
					.WhereHasSeries(queryParams.SeriesId)
					.WhereStatusIs(queryParams.EntryStatus)
					.WhereDateIsBetween(queryParams.AfterDate, queryParams.BeforeDate)
					.WhereInUserCollection(queryParams.UserId)
					.WhereHasTags(queryParams.TagIds, queryParams.ChildTags);

				var entries = q
					.OrderBy(queryParams.SortRule, LanguagePreference, queryParams.SortDirection)
					.Paged(queryParams.Paging)
					.ToArray()
					.Select(fac)
					.ToArray();

				var count = 0;

				if (queryParams.Paging != null && queryParams.Paging.GetTotalCount) {

					count = q.Count();

				}

				return new PartialFindResult<TResult>(entries, count);

			});

		}

		public PartialFindResult<ReleaseEventSeriesForApiContract> FindSeries(SearchTextQuery textQuery, PagingProperties paging,
			ContentLanguagePreference lang, ReleaseEventSeriesOptionalFields fields = ReleaseEventSeriesOptionalFields.None) {
			return FindSeries(s => new ReleaseEventSeriesForApiContract(s, lang, fields, imageUrlFactory), textQuery, paging);
		}

		public PartialFindResult<TResult> FindSeries<TResult>(Func<ReleaseEventSeries, TResult> fac, 
			SearchTextQuery textQuery, PagingProperties paging) {

			return HandleQuery(ctx => {

				var q = ctx.Query<ReleaseEventSeries>()
					.WhereNotDeleted()
					.WhereHasName(textQuery)
					.Paged(paging);

				var entries = q
					.OrderByEntryName(PermissionContext.LanguagePreference)
					.ToArray()
					.Select(fac)
					.ToArray();

				var count = paging.GetTotalCount ? q.Count() : 0;

				return PartialFindResult.Create(entries, count);

			});

		}

		public ReleaseEventDetailsContract GetDetails(int id) {

			return HandleQuery(ctx => {

				UserEventRelationshipType? eventAssociation = null;

				if (permissionContext.IsLoggedIn) {
					eventAssociation = ctx.Query<EventForUser>()
						.Where(e => e.ReleaseEvent.Id == id && e.User.Id == permissionContext.LoggedUserId)
						.Select(e => e.RelationshipType)
						.FirstOrDefault();
				}

				return new ReleaseEventDetailsContract(ctx.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference, PermissionContext, userIconFactory,
					new EntryTypeTags(ctx)) {	
					EventAssociationType = eventAssociation,
					LatestComments = new CommentQueries<ReleaseEventComment, ReleaseEvent>(
						ctx, PermissionContext, userIconFactory, entryLinkFactory).GetList(id, 3)
				};
			});

		}

		public EntryWithTagUsagesContract GetEntryWithTagUsages(int eventId) {

			return HandleQuery(session => {
				var releaseEvent = session.Load<ReleaseEvent>(eventId);
				return new EntryWithTagUsagesContract(releaseEvent, releaseEvent.Tags.ActiveUsages, LanguagePreference, PermissionContext);
			});

		}

		public ReleaseEventForEditContract GetEventForEdit(int id) {

			return HandleQuery(session => new ReleaseEventForEditContract(
				session.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference, PermissionContext, null) {
				AllSeries = session.Query<ReleaseEventSeries>().Select(s => new ReleaseEventSeriesContract(s, LanguagePreference, false)).ToArray()
			});

		}

		public ReleaseEventForApiContract GetOne(int id, ContentLanguagePreference lang, ReleaseEventOptionalFields fields) {
			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), lang, fields, imageUrlFactory));
		}
		
		public VenueForApiContract[] GetReleaseEventsByVenue() {

			return HandleQuery(session => {

				var allEvents = session.Query<ReleaseEvent>().Where(e => !e.Deleted).ToArray();
				var venues = session.Query<Venue>().Where(e => !e.Deleted).OrderByName(LanguagePreference).ToArray();

				var venueContracts = venues.Select(v => new VenueForApiContract(
					v,
					PermissionContext.LanguagePreference,
					VenueOptionalFields.AdditionalNames | VenueOptionalFields.Description | VenueOptionalFields.Events | VenueOptionalFields.Names | VenueOptionalFields.WebLinks));
				var ungrouped = allEvents.Where(e => e.Venue == null).OrderBy(e => e.TranslatedName[LanguagePreference]);

				return venueContracts.Append(new VenueForApiContract {
					Name = string.Empty,
					Events = ungrouped.Select(e => new ReleaseEventContract(e, LanguagePreference)).ToArray()
				}).ToArray();

			});

		}

		public ReleaseEventSeriesForApiContract GetOneSeries(int id, ContentLanguagePreference lang, ReleaseEventSeriesOptionalFields fields) {
			return repository.HandleQuery(ctx => new ReleaseEventSeriesForApiContract(ctx.Load<ReleaseEventSeries>(id), lang, fields, imageUrlFactory));
		}

		public ArchivedEventSeriesVersionDetailsContract GetSeriesVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session => {

				var contract = new ArchivedEventSeriesVersionDetailsContract(session.Load<ArchivedReleaseEventSeriesVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedReleaseEventSeriesVersion>(comparedVersionId) : null,
					PermissionContext);

				if (contract.Hidden) {
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return contract;

			});
		}

		public ArchivedEventVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session => {

				var contract = new ArchivedEventVersionDetailsContract(session.Load<ArchivedReleaseEventVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedReleaseEventVersion>(comparedVersionId) : null,
					PermissionContext);

				if (contract.Hidden) {
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return contract;

			});

		}

		public XDocument GetSeriesVersionXml(int id) {

			return HandleQuery(ctx => {

				var archivedVersion = ctx.Load<ArchivedReleaseEventSeriesVersion>(id);

				if (archivedVersion.Hidden) {
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return archivedVersion.Data;

			});

		}

		public XDocument GetVersionXml(int id) {

			return HandleQuery(ctx => {

				var archivedVersion = ctx.Load<ArchivedReleaseEventVersion>(id);

				if (archivedVersion.Hidden) {
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return archivedVersion.Data;

			});

		}

		public ReleaseEventContract[] List(EventSortRule sortRule, SortDirection sortDirection, bool includeSeries = false) {
			
			return repository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date.DateTime != null)
				.OrderBy(sortRule, LanguagePreference, sortDirection)
				.ToArray()
				.Select(e => new ReleaseEventContract(e, LanguagePreference, includeSeries))
				.ToArray());

		}

		public ReleaseEventForApiContract Load(int id, ReleaseEventOptionalFields fields) {

			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), LanguagePreference, fields, imageUrlFactory));

		}

		private void CreateTrashedEntry(IDatabaseContext ctx, ReleaseEvent releaseEvent, string notes) {

			var archived = new ArchivedEventContract(releaseEvent, new ReleaseEventDiff(true));
			var data = XmlHelper.SerializeToXml(archived);
			var trashed = new TrashedEntry(releaseEvent, data, GetLoggedUser(ctx), notes);

			ctx.Save(trashed);

		}

		private void CreateTrashedEntry(IDatabaseContext ctx, ReleaseEventSeries eventSeries, string notes) {

			var archived = new ArchivedEventSeriesContract(eventSeries, new ReleaseEventSeriesDiff(true));
			var data = XmlHelper.SerializeToXml(archived);
			var trashed = new TrashedEntry(eventSeries, data, GetLoggedUser(ctx), notes);

			ctx.Save(trashed);

		}

		public void MoveSeriesToTrash(int seriesId, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load<ReleaseEventSeries>(seriesId);

				PermissionContext.VerifyEntryDelete(entry);

				ctx.AuditLogger.SysLog(string.Format("moving {0} to trash", entry));

				CreateTrashedEntry(ctx, entry, notes);

				var allEvents = entry.AllEvents.ToArray();
				foreach (var ev in allEvents) {
					ev.SetSeries(null);
				}

				ctx.Delete(entry);

				ctx.AuditLogger.AuditLog(string.Format("moved {0} to trash", entry));

			});

		}

		public void MoveToTrash(int eventId, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(eventId);

				PermissionContext.VerifyEntryDelete(entry);

				ctx.AuditLogger.SysLog(string.Format("moving {0} to trash", entry));

				CreateTrashedEntry(ctx, entry, notes);

				entry.Series?.AllEvents.Remove(entry);

				foreach (var song in entry.AllSongs) {
					song.ReleaseEvent = null;
				}

				var ctxActivity = ctx.OfType<ReleaseEventActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == eventId).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.Delete(entry);

				ctx.AuditLogger.AuditLog(string.Format("moved {0} to trash", entry));

			});

		}

		public int RemoveTagUsage(long tagUsageId) {

			return new TagUsageQueries(PermissionContext).RemoveTagUsage<EventTagUsage, ReleaseEvent>(tagUsageId, repository);

		}

		public void Restore(int eventId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(ctx => {

				var ev = ctx.Load<ReleaseEvent>(eventId);

				ev.Deleted = false;

				ctx.Update(ev);

				Archive(ctx, ev, new ReleaseEventDiff(false), EntryEditEvent.Restored, string.Empty);

				ctx.AuditLogger.AuditLog(string.Format("restored {0}", ev));

			});

		}

		public void RestoreSeries(int eventId) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(ctx => {

				var ev = ctx.Load<ReleaseEventSeries>(eventId);

				ev.Deleted = false;

				ctx.Update(ev);

				Archive(ctx, ev, new ReleaseEventSeriesDiff(false), EntryEditEvent.Restored, string.Empty);

				ctx.AuditLogger.AuditLog(string.Format("restored {0}", ev));

			});

		}

		/// <summary>
		/// Updates or creates release event.
		/// 
		/// Album release event names will be updated as well if the name changed.
		/// </summary>
		/// <param name="contract">Updated contract. Cannot be null.</param>
		/// <returns>Updated release event data. Cannot be null.</returns>
		/// <exception cref="DuplicateEventNameException">If the event name is already in use.</exception>
		public async Task<ReleaseEventContract> Update(ReleaseEventForEditContract contract, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return await repository.HandleTransactionAsync(async session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					var diff = new ReleaseEventDiff();

					if (!contract.Series.IsNullOrDefault()) {
						var series = await session.LoadAsync<ReleaseEventSeries>(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber, contract.SeriesSuffix, 
							contract.DefaultNameLanguage, contract.CustomName);
						series.AllEvents.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.DefaultNameLanguage);
					}

					ev.Category = contract.Category;
					ev.EndDate = contract.EndDate;
					ev.SongList = await session.NullSafeLoadAsync<SongList>(contract.SongList);
					ev.Status = contract.Status;
					ev.SetVenue(await session.NullSafeLoadAsync<Venue>(contract.Venue));
					ev.VenueName = contract.VenueName;

					if (contract.SongList != null) {
						diff.SongList.Set();
					}

					if (contract.Venue != null) {
						diff.Venue.Set();
					}

					if (!string.IsNullOrEmpty(contract.VenueName)) {
						diff.VenueName.Set();
					}

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
					}

					var pvDiff = ev.PVs.Sync(contract.PVs, ev.CreatePV);
					await session.OfType<PVForAlbum>().SyncAsync(pvDiff);

					if (pvDiff.Changed)
						diff.PVs.Set();

					var artistDiff = await ev.SyncArtists(contract.Artists, artistId => session.LoadAsync<Artist>(artistId));

					if (artistDiff.Changed)
						diff.Artists.Set();

					await session.SaveAsync(ev);

					var namesChanged = new UpdateEventNamesQuery().UpdateNames(session, ev, contract.Series, contract.CustomName, contract.SeriesNumber, contract.SeriesSuffix, contract.Names);
					if (namesChanged) {
						await session.UpdateAsync(ev);
					}

					if (pictureData != null) {
						diff.MainPicture.Set();
						SaveImage(ev, pictureData);
						await session.UpdateAsync(ev);
					}

					var archived = Archive(session, ev, diff, EntryEditEvent.Created, string.Empty);
					await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), archived);

					await session.AuditLogger.AuditLogAsync(string.Format("created {0}", entryLinkFactory.CreateEntryLink(ev)));

					await followedArtistNotifier.SendNotificationsAsync(session, ev, ev.Artists.Where(a => a?.Artist != null).Select(a => a.Artist), PermissionContext.LoggedUser);

				} else {

					ev = await session.LoadAsync(contract.Id);
					permissionContext.VerifyEntryEdit(ev);

					var diff = new ReleaseEventDiff(DoSnapshot(ev, session));

					if (ev.Category != contract.Category)
						diff.Category.Set();

					if (!ev.Date.Equals(contract.Date) || !ev.EndDate.Equals(contract.EndDate))
						diff.Date.Set();

					if (ev.Description != contract.Description)
						diff.Description.Set();

					var inheritedLanguage = ev.Series == null || contract.CustomName ? contract.DefaultNameLanguage : ev.Series.TranslatedName.DefaultLanguage;
					if (ev.TranslatedName.DefaultLanguage != inheritedLanguage) {
						diff.OriginalName.Set();
					}

					var namesChanged = new UpdateEventNamesQuery().UpdateNames(session, ev, contract.Series, contract.CustomName, contract.SeriesNumber, contract.SeriesSuffix, contract.Names);

					if (namesChanged) {
						diff.Names.Set();
					}

					if (!ev.Series.NullSafeIdEquals(contract.Series)) {
						diff.Series.Set();
					}

					if (ev.SeriesNumber != contract.SeriesNumber)
						diff.SeriesNumber.Set();

					if (ev.SeriesSuffix != contract.SeriesSuffix)
						diff.SeriesSuffix.Set();

					if (!ev.SongList.NullSafeIdEquals(contract.SongList)) {
						diff.SongList.Set();
					}

					if (ev.Status != contract.Status)
						diff.Status.Set();
					
					if (!ev.Venue.NullSafeIdEquals(contract.Venue)) {
						diff.Venue.Set();
					}

					if (!string.Equals(ev.VenueName, contract.VenueName)) {
						diff.VenueName.Set();
					}

					ev.SetSeries(await session.NullSafeLoadAsync<ReleaseEventSeries>(contract.Series));
					ev.Category = contract.Category;
					ev.CustomName = contract.CustomName;
					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.EndDate = contract.EndDate > contract.Date ? contract.EndDate : null;
					ev.SeriesNumber = contract.SeriesNumber;
					ev.SeriesSuffix = contract.SeriesSuffix;
					ev.SongList = await session.NullSafeLoadAsync<SongList>(contract.SongList);
					ev.Status = contract.Status;
					ev.TranslatedName.DefaultLanguage = inheritedLanguage;
					ev.SetVenue(await session.NullSafeLoadAsync<Venue>(contract.Venue));
					ev.VenueName = contract.VenueName;

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						await session.OfType<ReleaseEventWebLink>().SyncAsync(weblinksDiff);
					}

					var pvDiff = ev.PVs.Sync(contract.PVs, ev.CreatePV);
					await session.OfType<PVForAlbum>().SyncAsync(pvDiff);

					if (pvDiff.Changed)
						diff.PVs.Set();

					var artistDiff = await ev.SyncArtists(contract.Artists, artistId => session.LoadAsync<Artist>(artistId));

					if (artistDiff.Changed)
						diff.Artists.Set();

					if (pictureData != null) {
						diff.MainPicture.Set();
						SaveImage(ev, pictureData);
					}

					await session.UpdateAsync(ev);

					var archived = Archive(session, ev, diff, EntryEditEvent.Updated, string.Empty);
					await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), archived);

					var logStr = string.Format("updated properties for {0} ({1})", entryLinkFactory.CreateEntryLink(ev), diff.ChangedFieldsString);
					await session.AuditLogger.AuditLogAsync(logStr);

					var newSongCutoff = TimeSpan.FromHours(1);
					if (artistDiff.Added.Any() && ev.CreateDate >= DateTime.Now - newSongCutoff) {

						var addedArtists = artistDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

						if (addedArtists.Any()) {
							await followedArtistNotifier.SendNotificationsAsync(session, ev, addedArtists, PermissionContext.LoggedUser);
						}

					}

				}

				return new ReleaseEventContract(ev, LanguagePreference);

			});

		}

		private PictureDataContract SaveImage(IEntryImageInformation entry, EntryPictureFileContract pictureData) {

			if (pictureData == null) return null;

			var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);

			pictureData.Id = entry.Id;
			pictureData.EntryType = entry.EntryType;
			var thumbGenerator = new ImageThumbGenerator(imagePersister);
			thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ReleaseEventSeries.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);
			return parsed;

		}

		private void SaveImage(ReleaseEventSeries series, EntryPictureFileContract pictureData) {

			var parsed = SaveImage((IEntryImageInformation)series, pictureData);
			series.PictureMime = parsed.Mime;

		}

		private void SaveImage(ReleaseEvent ev, EntryPictureFileContract pictureData) {

			var parsed = SaveImage((IEntryImageInformation)ev, pictureData);
			ev.PictureMime = parsed.Mime;

		}

		public int UpdateSeries(ReleaseEventSeriesForEditContract contract, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return HandleTransaction(session => {

				ReleaseEventSeries series;

				if (contract.Id == 0) {

					series = new ReleaseEventSeries(contract.DefaultNameLanguage, contract.Names, contract.Description) {
						Category = contract.Category,
						Status = contract.Status
					};
					session.Save(series);

					var diff = new ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields.OriginalName | ReleaseEventSeriesEditableFields.Names);

					diff.Description.Set(!string.IsNullOrEmpty(contract.Description));

					var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					if (pictureData != null) {
						diff.Picture.Set();
						SaveImage(series, pictureData);
						session.Update(series);
					}

					session.Update(series);

					Archive(session, series, diff, EntryEditEvent.Created, string.Empty);

					AuditLog(string.Format("created {0}", entryLinkFactory.CreateEntryLink(series)), session);

				} else {

					series = session.Load<ReleaseEventSeries>(contract.Id);
					permissionContext.VerifyEntryEdit(series);
					var diff = new ReleaseEventSeriesDiff(DoSnapshot(series, session));

					if (series.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage) {
						series.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
						diff.OriginalName.Set();
					}

					var nameDiff = series.Names.Sync(contract.Names, series);
					session.Sync(nameDiff);

					if (nameDiff.Changed) {
						diff.Names.Set();
					}


					if (series.Category != contract.Category) {
						diff.Category.Set();
						series.Category = contract.Category;
					}

					if (series.Description != contract.Description) {
						diff.Description.Set();
						series.Description = contract.Description;
					}

					if (series.Status != contract.Status) {
						diff.Status.Set();
						series.Status = contract.Status;
					}

					if (pictureData != null) {
						diff.Picture.Set();
						SaveImage(series, pictureData);
					}

					var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						session.Sync(weblinksDiff);
					}

					session.Update(series);

					if (diff.Names.IsChanged || diff.OriginalName.IsChanged) {
						var eventNamesQuery = new UpdateEventNamesQuery();
						foreach (var ev in series.Events.Where(e => !e.CustomName)) {
							eventNamesQuery.UpdateNames(session, ev, series, ev.CustomName, ev.SeriesNumber, ev.SeriesSuffix, ev.Names);
							ev.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
							session.Update(ev);
						}
					}

					Archive(session, series, diff, EntryEditEvent.Updated, string.Empty);

					AuditLog(string.Format("updated {0}", entryLinkFactory.CreateEntryLink(series)), session);

				}

				return series.Id;

			});

		}

	}

}