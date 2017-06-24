using System;
using System.Linq;
using System.Xml.Linq;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
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
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
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
		private readonly IEntryThumbPersister imagePersister;
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

		private void CheckDuplicateName(IDatabaseContext<ReleaseEvent> ctx, ReleaseEvent ev) {

			var duplicate = ctx.Query<EventName>().FirstOrDefault(e => e.Entry.Id != ev.Id && ev.Names.AllValues.Contains(e.Value));

			if (duplicate != null) {
				throw new DuplicateEventNameException(duplicate.Value);
			}

		}

		public EventQueries(IEventRepository eventRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext,
			IEntryThumbPersister imagePersister, IUserIconFactory userIconFactory, IEnumTranslations enumTranslations)
			: base(eventRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;
			this.enumTranslations = enumTranslations;

		}

		public bool CreateReport(int eventId, EventReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory, report => report.Entry.Id == eventId,
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

		public PartialFindResult<TResult> FindSeries<TResult>(Func<ReleaseEventSeries, TResult> fac, 
			SearchTextQuery textQuery, PagingProperties paging) {

			return HandleQuery(ctx => {

				var q = ctx.Query<ReleaseEventSeries>()
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

				return new ReleaseEventDetailsContract(ctx.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference, PermissionContext, userIconFactory) {	
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

		public ReleaseEventForApiContract GetOne(int id, ContentLanguagePreference lang, ReleaseEventOptionalFields fields, bool ssl) {
			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), lang, fields, imagePersister, ssl));
		}

		public ArchivedEventVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedEventVersionDetailsContract(session.Load<ArchivedReleaseEventVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedReleaseEventVersion>(comparedVersionId) : null,
					PermissionContext.LanguagePreference));

		}

		public XDocument GetVersionXml(int id) {
			return HandleQuery(ctx => ctx.Load<ArchivedReleaseEventVersion>(id).Data);
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

			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), LanguagePreference, fields, imagePersister, true));

		}

		public void MoveToTrash(int eventId) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(eventId);

				PermissionContext.VerifyEntryDelete(entry);

				ctx.AuditLogger.SysLog(string.Format("moving {0} to trash", entry));

				foreach (var song in entry.AllSongs)
				{
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
		public ReleaseEventContract Update(ReleaseEventForEditContract contract, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return repository.HandleTransaction(session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					var diff = new ReleaseEventDiff();

					if (!contract.Series.IsNullOrDefault()) {
						var series = session.OfType<ReleaseEventSeries>().Load(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber, contract.SeriesSuffix, 
							contract.DefaultNameLanguage, contract.Names, contract.CustomName);
						series.AllEvents.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.DefaultNameLanguage, contract.Names);
					}

					ev.Category = contract.Category;
					ev.SongList = session.NullSafeLoad<SongList>(contract.SongList);
					ev.Status = contract.Status;
					ev.Venue = contract.Venue;

					if (contract.SongList != null) {
						diff.SongList.Set();
					}

					if (!string.IsNullOrEmpty(contract.Venue)) {
						diff.Venue.Set();
					}

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
					}

					var pvDiff = ev.PVs.Sync(contract.PVs, ev.CreatePV);
					session.OfType<PVForAlbum>().Sync(pvDiff);

					if (pvDiff.Changed)
						diff.PVs.Set();

					var artistDiff = ev.SyncArtists(contract.Artists, artistId => session.Load<Artist>(artistId));

					if (artistDiff.Changed)
						diff.Artists.Set();

					CheckDuplicateName(session, ev);

					session.Save(ev);

					if (pictureData != null) {
						diff.MainPicture.Set();
						SaveImage(ev, pictureData);
						session.Update(ev);
					}

					var archived = Archive(session, ev, diff, EntryEditEvent.Created, string.Empty);
					AddEntryEditedEntry(session.OfType<ActivityEntry>(), archived);

					session.AuditLogger.AuditLog(string.Format("created {0}", entryLinkFactory.CreateEntryLink(ev)));

				} else {

					ev = session.Load(contract.Id);
					permissionContext.VerifyEntryEdit(ev);
					var diff = new ReleaseEventDiff(DoSnapshot(ev, session));

					if (ev.Category != contract.Category)
						diff.Category.Set();

					if (!ev.Date.Equals(contract.Date))
						diff.Date.Set();

					if (ev.Description != contract.Description)
						diff.Description.Set();

					if (ev.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage) {
						diff.OriginalName.Set();
					}

					var nameDiff = ev.Names.Sync(contract.Names, ev);
					session.Sync(nameDiff);

					if (nameDiff.Changed) {
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

					if (!string.Equals(ev.Venue, contract.Venue)) {
						diff.Venue.Set();
					}

					ev.Series = session.NullSafeLoad<ReleaseEventSeries>(contract.Series);
					ev.Category = contract.Category;
					ev.CustomName = contract.CustomName;
					ev.Date = contract.Date;
					ev.Description = contract.Description;
					ev.SeriesNumber = contract.SeriesNumber;
					ev.SeriesSuffix = contract.SeriesSuffix;
					ev.SongList = session.NullSafeLoad<SongList>(contract.SongList);
					ev.Status = contract.Status;
					ev.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
					ev.Venue = contract.Venue;
					ev.UpdateNameFromSeries();

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					var pvDiff = ev.PVs.Sync(contract.PVs, ev.CreatePV);
					session.OfType<PVForAlbum>().Sync(pvDiff);

					if (pvDiff.Changed)
						diff.PVs.Set();

					var artistDiff = ev.SyncArtists(contract.Artists, artistId => session.Load<Artist>(artistId));

					if (artistDiff.Changed)
						diff.Artists.Set();

					CheckDuplicateName(session, ev);

					if (pictureData != null) {
						diff.MainPicture.Set();
						SaveImage(ev, pictureData);
					}

					session.Update(ev);

					var archived = Archive(session, ev, diff, EntryEditEvent.Updated, string.Empty);
					AddEntryEditedEntry(session.OfType<ActivityEntry>(), archived);

					var logStr = string.Format("updated properties for {0} ({1})", entryLinkFactory.CreateEntryLink(ev), diff.ChangedFieldsString);
					session.AuditLogger.AuditLog(logStr);

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
					var diff = new ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields.Nothing);

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
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

					session.Update(series);

					if (diff.Names.IsChanged || diff.OriginalName.IsChanged) {
						foreach (var ev in series.Events.Where(e => !e.CustomName)) {
							ev.UpdateNameFromSeries();
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