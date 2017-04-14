using System;
using System.Linq;
using NHibernate;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ExtLinks;
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

namespace VocaDb.Model.Database.Queries {

	public class EventQueries : QueriesBase<IEventRepository, ReleaseEvent> {

		private readonly IEntryLinkFactory entryLinkFactory;
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

			var hasDuplicate = ctx.Query().Any(e => e.Id != ev.Id && e.Name == ev.Name);

			if (hasDuplicate) {
				throw new DuplicateEventNameException(ev.Name);
			}

		}

		public EventQueries(IEventRepository eventRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext,
			IEntryThumbPersister imagePersister, IUserIconFactory userIconFactory)
			: base(eventRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.imagePersister = imagePersister;
			this.userIconFactory = userIconFactory;

		}

		public void Delete(int eventId) {
			
			repository.HandleTransaction(ctx => {
				
				var entry = ctx.Load(eventId);

				ctx.AuditLogger.SysLog(string.Format("deleting {0}", entry));

				foreach (var song in entry.AllSongs) {
					song.ReleaseEvent = null;
				}

				var ctxActivity = ctx.OfType<ReleaseEventActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == eventId).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.Delete(entry);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}

		public PartialFindResult<TResult> Find<TResult>(Func<ReleaseEvent, TResult> fac, EventQueryParams queryParams) {

			return HandleQuery(ctx => {

				var q = ctx.Query()
					.WhereHasName(queryParams.TextQuery)
					.WhereHasSeries(queryParams.SeriesId)
					.WhereDateIsBetween(queryParams.AfterDate, queryParams.BeforeDate);

				var entries = q
					.OrderBy(queryParams.SortRule, queryParams.SortDirection)
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
					.OrderBy(s => s.Name)
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

				return new ReleaseEventDetailsContract(ctx.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference, userIconFactory) {	
					EventAssociationType = eventAssociation,
					LatestComments = new CommentQueries<ReleaseEventComment, ReleaseEvent>(
						ctx, PermissionContext, userIconFactory, entryLinkFactory).GetList(id, 3)
				};
			});

		}

		public ReleaseEventContract[] List(EventSortRule sortRule, SortDirection sortDirection, bool includeSeries = false) {
			
			return repository.HandleQuery(ctx => ctx
				.Query()
				.Where(e => e.Date.DateTime != null)
				.OrderBy(sortRule, sortDirection)
				.ToArray()
				.Select(e => new ReleaseEventContract(e, includeSeries))
				.ToArray());

		}

		public ReleaseEventForApiContract Load(int id, ReleaseEventOptionalFields fields) {

			return repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), fields, imagePersister, true));

		}

		/// <summary>
		/// Updates or creates release event.
		/// 
		/// Album release event names will be updated as well if the name changed.
		/// </summary>
		/// <param name="contract">Updated contract. Cannot be null.</param>
		/// <returns>Updated release event data. Cannot be null.</returns>
		public ReleaseEventContract Update(ReleaseEventDetailsContract contract, EntryPictureFileContract pictureData) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return repository.HandleTransaction(session => {

				ReleaseEvent ev;

				if (contract.Id == 0) {

					var diff = new ReleaseEventDiff();

					if (!contract.Series.IsNullOrDefault()) {
						var series = session.OfType<ReleaseEventSeries>().Load(contract.Series.Id);
						ev = new ReleaseEvent(contract.Description, contract.Date, series, contract.SeriesNumber, contract.SeriesSuffix, 
							contract.Name, contract.CustomName);
						series.Events.Add(ev);
					} else {
						ev = new ReleaseEvent(contract.Description, contract.Date, contract.Name);
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

					if (ev.Name != contract.Name && (contract.Series == null || contract.CustomName))
						diff.Name.Set();

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
					ev.Name = contract.Name;
					ev.SeriesNumber = contract.SeriesNumber;
					ev.SeriesSuffix = contract.SeriesSuffix;
					ev.SongList = session.NullSafeLoad<SongList>(contract.SongList);
					ev.Status = contract.Status;
					ev.Venue = contract.Venue;
					ev.UpdateNameFromSeries();

					var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
					}

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

				return new ReleaseEventContract(ev);

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

			PermissionContext.VerifyPermission(PermissionToken.ManageEventSeries);

			return HandleTransaction(session => {

				ReleaseEventSeries series;

				if (contract.Id == 0) {

					series = new ReleaseEventSeries(contract.Name, contract.Description, contract.Aliases) {
						Category = contract.Category
					};
					session.Save(series);

					var diff = new ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields.Name);

					diff.Description.Set(!string.IsNullOrEmpty(contract.Description));
					diff.Names.Set(contract.Aliases.Any());

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
					var diff = new ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields.Nothing);

					if (series.Name != contract.Name) {
						diff.Name.Set();
						series.Name = contract.Name;
					}

					if (series.Category != contract.Category) {
						diff.Category.Set();
						series.Category = contract.Category;
					}

					if (series.Description != contract.Description) {
						diff.Description.Set();
						series.Description = contract.Description;
					}

					if (series.UpdateAliases(contract.Aliases)) {
						diff.Names.Set();
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

					Archive(session, series, diff, EntryEditEvent.Updated, string.Empty);

					AuditLog(string.Format("updated {0}", entryLinkFactory.CreateEntryLink(series)), session);

				}

				return series.Id;

			});

		}

	}

}