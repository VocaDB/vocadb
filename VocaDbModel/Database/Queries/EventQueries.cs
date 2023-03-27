#nullable disable

using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.Database.Queries.Partial;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Comments;
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
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.Events;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries;

public class EventQueries : QueriesBase<IEventRepository, ReleaseEvent>
{
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly IEnumTranslations _enumTranslations;
	private readonly IFollowedArtistNotifier _followedArtistNotifier;
	private readonly IAggregatedEntryImageUrlFactory _imageUrlFactory;
	private readonly IEntryThumbPersister _imagePersister;
	private readonly IUserMessageMailer _mailer;
	private readonly IUserIconFactory _userIconFactory;
	private readonly IDiscordWebhookNotifier _discordWebhookNotifier;

	private ArchivedReleaseEventVersion Archive(IDatabaseContext<ReleaseEvent> ctx, ReleaseEvent releaseEvent, ReleaseEventDiff diff, EntryEditEvent reason, string notes)
	{
		var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(_permissionContext);
		var archived = ArchivedReleaseEventVersion.Create(releaseEvent, diff, agentLoginData, reason, notes);
		ctx.Save(archived);
		return archived;
	}

	private ArchivedReleaseEventSeriesVersion Archive(IDatabaseContext<ReleaseEvent> ctx, ReleaseEventSeries releaseEvent, ReleaseEventSeriesDiff diff, EntryEditEvent reason, string notes)
	{
		var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(_permissionContext);
		var archived = ArchivedReleaseEventSeriesVersion.Create(releaseEvent, diff, agentLoginData, reason, notes);
		ctx.Save(archived);
		return archived;
	}

	public EventQueries(
		IEventRepository eventRepository,
		IEntryLinkFactory entryLinkFactory,
		IUserPermissionContext permissionContext,
		IEntryThumbPersister imagePersister,
		IUserIconFactory userIconFactory,
		IEnumTranslations enumTranslations,
		IUserMessageMailer mailer,
		IFollowedArtistNotifier followedArtistNotifier,
		IAggregatedEntryImageUrlFactory imageUrlFactory,
		IDiscordWebhookNotifier discordWebhookNotifier
	)
		: base(eventRepository, permissionContext)
	{
		_entryLinkFactory = entryLinkFactory;
		_imagePersister = imagePersister;
		_userIconFactory = userIconFactory;
		_enumTranslations = enumTranslations;
		_mailer = mailer;
		_followedArtistNotifier = followedArtistNotifier;
		_imageUrlFactory = imageUrlFactory;
		_discordWebhookNotifier = discordWebhookNotifier;
	}

#nullable enable
	public Task<(bool created, int reportId)> CreateReport(int eventId, EventReportType reportType, string hostname, string notes, int? versionNumber)
	{
		ParamIs.NotNull(() => hostname);
		ParamIs.NotNull(() => notes);

		return HandleTransactionAsync(ctx =>
		{
			return new Service.Queries.EntryReportQueries().CreateReport(
				ctx,
				PermissionContext,
				_entryLinkFactory,
				(song, reporter, notesTruncated) => new EventReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
				() => reportType != EventReportType.Other ? _enumTranslations.Translation(reportType) : null,
				eventId,
				reportType,
				hostname,
				notes,
				_discordWebhookNotifier,
				EventReport.ReportTypesWithRequiredNotes
			);
		});
	}
#nullable disable

	public void Delete(int eventId, string notes)
	{
		_permissionContext.VerifyManageDatabase();

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load(eventId);

			PermissionContext.VerifyEntryDelete(entry);

			entry.Deleted = true;
			ctx.Update(entry);

			Archive(ctx, entry, new ReleaseEventDiff(false), EntryEditEvent.Deleted, notes);

			ctx.AuditLogger.AuditLog($"deleted {entry}");
		});
	}

	public void DeleteSeries(int id, string notes)
	{
		_permissionContext.VerifyManageDatabase();

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load<ReleaseEventSeries>(id);

			PermissionContext.VerifyEntryDelete(entry);

			entry.Deleted = true;
			ctx.Update(entry);

			Archive(ctx, entry, new ReleaseEventSeriesDiff(false), EntryEditEvent.Deleted, notes);

			ctx.AuditLogger.AuditLog($"deleted {entry}");
		});
	}

	public PartialFindResult<TResult> Find<TResult>(Func<ReleaseEvent, TResult> fac, EventQueryParams queryParams)
	{
		return HandleQuery(ctx =>
		{
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

			if (queryParams.Paging != null && queryParams.Paging.GetTotalCount)
			{
				count = q.Count();
			}

			return new PartialFindResult<TResult>(entries, count);
		});
	}

	public PartialFindResult<ReleaseEventSeriesForApiContract> FindSeries(
		SearchTextQuery textQuery,
		PagingProperties paging,
		ContentLanguagePreference lang,
		ReleaseEventSeriesOptionalFields fields = ReleaseEventSeriesOptionalFields.None
	)
	{
		return FindSeries(s => new ReleaseEventSeriesForApiContract(s, lang, PermissionContext, fields, _imageUrlFactory), textQuery, paging);
	}

	public PartialFindResult<TResult> FindSeries<TResult>(Func<ReleaseEventSeries, TResult> fac,
		SearchTextQuery textQuery, PagingProperties paging)
	{
		return HandleQuery(ctx =>
		{
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

	public ReleaseEventDetailsForApiContract GetDetails(int id)
	{
		return HandleQuery(ctx =>
		{
			UserEventRelationshipType? eventAssociation = null;

			if (_permissionContext.IsLoggedIn)
			{
				eventAssociation = ctx.Query<EventForUser>()
					.Where(e => e.ReleaseEvent.Id == id && e.User.Id == _permissionContext.LoggedUserId)
					.Select(e => e.RelationshipType)
					.FirstOrDefault();
			}

			return new ReleaseEventDetailsForApiContract(
				releaseEvent: ctx.Load<ReleaseEvent>(id),
				languagePreference: PermissionContext.LanguagePreference,
				userContext: PermissionContext,
				userIconFactory: _userIconFactory,
				entryTypeTags: new EntryTypeTags(ctx),
				eventAssociationType: eventAssociation,
				latestComments: new CommentQueries<ReleaseEventComment, ReleaseEvent>(
					ctx: ctx,
					permissionContext: PermissionContext,
					userIconFactory: _userIconFactory,
					entryLinkFactory: _entryLinkFactory
				).GetList(id, 3),
				thumbPersister: _imageUrlFactory
			);
		});
	}

	public EntryWithTagUsagesContract GetEntryWithTagUsages(int eventId)
	{
		return HandleQuery(session =>
		{
			var releaseEvent = session.Load<ReleaseEvent>(eventId);
			return new EntryWithTagUsagesContract(releaseEvent, releaseEvent.Tags.ActiveUsages, LanguagePreference, PermissionContext, _userIconFactory);
		});
	}

	public ReleaseEventForEditContract GetEventForEdit(int id)
	{
		return HandleQuery(session => new ReleaseEventForEditContract(
			session.Load<ReleaseEvent>(id), PermissionContext.LanguagePreference, PermissionContext, null)
		{
			AllSeries = session.Query<ReleaseEventSeries>().Select(s => new ReleaseEventSeriesContract(s, LanguagePreference, false)).ToArray()
		});
	}

#nullable enable
	public ReleaseEventForEditForApiContract GetEventForEditForApi(int id)
	{
		return HandleQuery(
			session => new ReleaseEventForEditForApiContract(
				releaseEvent: session.Load<ReleaseEvent>(id),
				languagePreference: PermissionContext.LanguagePreference,
				permissionContext: PermissionContext,
				allSeries: session.Query<ReleaseEventSeries>()
					.Select(s => new ReleaseEventSeriesForApiContract(
						s,
						LanguagePreference,
						PermissionContext,
						ReleaseEventSeriesOptionalFields.None,
						null
					))
					.ToArray(),
				thumbPersister: _imageUrlFactory
			)
		);
	}
#nullable disable

	public ReleaseEventForApiContract GetOne(int id, ContentLanguagePreference lang, ReleaseEventOptionalFields fields)
	{
		return _repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), lang, PermissionContext, fields, _imageUrlFactory));
	}

	public VenueForApiContract[] GetReleaseEventsByVenue()
	{
		return HandleQuery(session =>
		{
			var allEvents = session.Query<ReleaseEvent>().Where(e => !e.Deleted).ToArray();
			var venues = session.Query<Venue>().Where(e => !e.Deleted).OrderByName(LanguagePreference).ToArray();

			var venueContracts = venues.Select(v => new VenueForApiContract(
				v,
				PermissionContext.LanguagePreference,
				VenueOptionalFields.AdditionalNames | VenueOptionalFields.Description | VenueOptionalFields.Events | VenueOptionalFields.Names | VenueOptionalFields.WebLinks));
			var ungrouped = allEvents.Where(e => e.Venue == null).OrderBy(e => e.TranslatedName[LanguagePreference]);

			return venueContracts.Append(new VenueForApiContract
			{
				Name = string.Empty,
				Events = ungrouped.Select(e => new ReleaseEventContract(e, LanguagePreference)).ToArray()
			}).ToArray();
		});
	}

	public ReleaseEventSeriesForApiContract GetOneSeries(int id, ContentLanguagePreference lang, ReleaseEventSeriesOptionalFields fields)
	{
		return _repository.HandleQuery(ctx => new ReleaseEventSeriesForApiContract(ctx.Load<ReleaseEventSeries>(id), lang, PermissionContext, fields, _imageUrlFactory));
	}

	[Obsolete]
	public ArchivedEventSeriesVersionDetailsContract GetSeriesVersionDetails(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedEventSeriesVersionDetailsContract(session.Load<ArchivedReleaseEventSeriesVersion>(id),
				comparedVersionId != 0 ? session.Load<ArchivedReleaseEventSeriesVersion>(comparedVersionId) : null,
				PermissionContext, _userIconFactory);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

#nullable enable
	public ArchivedEventSeriesVersionDetailsForApiContract GetSeriesVersionDetailsForApi(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedEventSeriesVersionDetailsForApiContract(
				archived: session.Load<ArchivedReleaseEventSeriesVersion>(id),
				comparedVersion: comparedVersionId != 0 ? session.Load<ArchivedReleaseEventSeriesVersion>(comparedVersionId) : null,
				permissionContext: PermissionContext,
				userIconFactory: _userIconFactory
			);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}
#nullable disable

	[Obsolete]
	public ArchivedEventVersionDetailsContract GetVersionDetails(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedEventVersionDetailsContract(session.Load<ArchivedReleaseEventVersion>(id),
				comparedVersionId != 0 ? session.Load<ArchivedReleaseEventVersion>(comparedVersionId) : null,
				PermissionContext, _userIconFactory);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

#nullable enable
	public ArchivedEventVersionDetailsForApiContract GetVersionDetailsForApi(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedEventVersionDetailsForApiContract(
				archived: session.Load<ArchivedReleaseEventVersion>(id),
				comparedVersion: comparedVersionId != 0 ? session.Load<ArchivedReleaseEventVersion>(comparedVersionId) : null,
				permissionContext: PermissionContext,
				userIconFactory: _userIconFactory
			);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

	public ReleaseEventForApiContract[] List(EventSortRule sortRule, SortDirection sortDirection, bool includeSeries = false)
	{
		return _repository.HandleQuery(ctx => ctx
			.Query()
			.Where(e => e.Date.DateTime != null)
			.OrderBy(sortRule, LanguagePreference, sortDirection)
			.ToArray()
			.Select(e => new ReleaseEventForApiContract(
				rel: e,
				languagePreference: LanguagePreference,
				PermissionContext,
				fields: includeSeries ? ReleaseEventOptionalFields.Series : ReleaseEventOptionalFields.None,
				thumbPersister: null
			))
			.ToArray()
		);
	}
#nullable disable

	public ReleaseEventForApiContract Load(int id, ReleaseEventOptionalFields fields)
	{
		return _repository.HandleQuery(ctx => new ReleaseEventForApiContract(ctx.Load(id), LanguagePreference, PermissionContext, fields, _imageUrlFactory));
	}

	private void CreateTrashedEntry(IDatabaseContext ctx, ReleaseEvent releaseEvent, string notes)
	{
		var archived = new ArchivedEventContract(releaseEvent, new ReleaseEventDiff(true));
		var data = XmlHelper.SerializeToXml(archived);
		var trashed = new TrashedEntry(releaseEvent, data, GetLoggedUser(ctx), notes);

		ctx.Save(trashed);
	}

	private void CreateTrashedEntry(IDatabaseContext ctx, ReleaseEventSeries eventSeries, string notes)
	{
		var archived = new ArchivedEventSeriesContract(eventSeries, new ReleaseEventSeriesDiff(true));
		var data = XmlHelper.SerializeToXml(archived);
		var trashed = new TrashedEntry(eventSeries, data, GetLoggedUser(ctx), notes);

		ctx.Save(trashed);
	}

	public void MoveSeriesToTrash(int seriesId, string notes)
	{
		PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load<ReleaseEventSeries>(seriesId);

			PermissionContext.VerifyEntryDelete(entry);

			ctx.AuditLogger.SysLog($"moving {entry} to trash");

			CreateTrashedEntry(ctx, entry, notes);

			var allEvents = entry.AllEvents.ToArray();
			foreach (var ev in allEvents)
			{
				ev.SetSeries(null);
			}

			ctx.Delete(entry);

			ctx.AuditLogger.AuditLog($"moved {entry} to trash");
		});
	}

	public void MoveToTrash(int eventId, string notes)
	{
		PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load(eventId);

			PermissionContext.VerifyEntryDelete(entry);

			ctx.AuditLogger.SysLog($"moving {entry} to trash");

			CreateTrashedEntry(ctx, entry, notes);

			entry.Series?.AllEvents.Remove(entry);

			foreach (var song in entry.AllSongs)
			{
				song.ReleaseEvent = null;
			}

			var ctxActivity = ctx.OfType<ReleaseEventActivityEntry>();
			var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == eventId).ToArray();

			foreach (var activityEntry in activityEntries)
				ctxActivity.Delete(activityEntry);

			ctx.Delete(entry);

			ctx.AuditLogger.AuditLog($"moved {entry} to trash");
		});
	}

	public int RemoveTagUsage(long tagUsageId)
	{
		return new TagUsageQueries(PermissionContext).RemoveTagUsage<EventTagUsage, ReleaseEvent>(tagUsageId, _repository);
	}

	public void Restore(int eventId)
	{
		PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

		HandleTransaction(ctx =>
		{
			var ev = ctx.Load<ReleaseEvent>(eventId);

			ev.Deleted = false;

			ctx.Update(ev);

			Archive(ctx, ev, new ReleaseEventDiff(false), EntryEditEvent.Restored, string.Empty);

			ctx.AuditLogger.AuditLog($"restored {ev}");
		});
	}

	public void RestoreSeries(int eventId)
	{
		PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

		HandleTransaction(ctx =>
		{
			var ev = ctx.Load<ReleaseEventSeries>(eventId);

			ev.Deleted = false;

			ctx.Update(ev);

			Archive(ctx, ev, new ReleaseEventSeriesDiff(false), EntryEditEvent.Restored, string.Empty);

			ctx.AuditLogger.AuditLog($"restored {ev}");
		});
	}

#nullable enable
	/// <summary>
	/// Updates or creates release event.
	/// 
	/// Album release event names will be updated as well if the name changed.
	/// </summary>
	/// <param name="contract">Updated contract. Cannot be null.</param>
	/// <returns>Updated release event data. Cannot be null.</returns>
	/// <exception cref="DuplicateEventNameException">If the event name is already in use.</exception>
	public async Task<ReleaseEventContract> Update(ReleaseEventForEditForApiContract contract, EntryPictureFileContract? pictureData)
	{
		ParamIs.NotNull(() => contract);

		PermissionContext.VerifyManageDatabase();

		return await _repository.HandleTransactionAsync(async session =>
		{
			ReleaseEvent ev;

			if (contract.Id == 0)
			{
				var diff = new ReleaseEventDiff();

				if (!contract.Series.IsNullOrDefault())
				{
					var series = await session.LoadAsync<ReleaseEventSeries>(contract.Series.Id);
					ev = new ReleaseEvent(
						description: contract.Description,
						date: contract.Date,
						series: series,
						seriesNumber: contract.SeriesNumber,
						seriesSuffix: contract.SeriesSuffix,
						defaultNameLanguage: contract.DefaultNameLanguage,
						customName: contract.CustomName
					);
					series.AllEvents.Add(ev);
				}
				else
				{
					ev = new ReleaseEvent(contract.Description, contract.Date, contract.DefaultNameLanguage);
				}

				ev.Category = contract.Category;
				ev.EndDate = contract.EndDate;
				ev.SongList = await session.NullSafeLoadAsync<SongList>(contract.SongList);
				ev.Status = contract.Status;
				ev.SetVenue(await session.NullSafeLoadAsync<Venue>(contract.Venue));
				ev.VenueName = contract.VenueName;

				if (contract.SongList != null)
				{
					diff.SongList.Set();
				}

				if (contract.Venue != null)
				{
					diff.Venue.Set();
				}

				if (!string.IsNullOrEmpty(contract.VenueName))
				{
					diff.VenueName.Set();
				}

				var weblinksDiff = WebLink.Sync(ev.WebLinks, contract.WebLinks, ev);

				if (weblinksDiff.Changed)
				{
					diff.WebLinks.Set();
				}

				var pvDiff = ev.PVs.Sync(contract.PVs, ev.CreatePV);

				if (pvDiff.Changed)
					diff.PVs.Set();

				var artistDiff = await ev.SyncArtists(contract.Artists, artistId => session.LoadAsync<Artist>(artistId));

				if (artistDiff.Changed)
					diff.Artists.Set();

				await session.SaveAsync(ev);

				var namesChanged = new UpdateEventNamesQuery().UpdateNames(
					session,
					ev,
					seriesLink: contract.Series,
					customName: contract.CustomName,
					seriesNumber: contract.SeriesNumber,
					seriesSuffix: contract.SeriesSuffix,
					nameContracts: contract.Names
				);
				if (namesChanged)
				{
					await session.UpdateAsync(ev);
				}

				if (pictureData != null)
				{
					diff.MainPicture.Set();
					SaveImage(ev, pictureData);
					await session.UpdateAsync(ev);
				}

				var archived = Archive(session, ev, diff, EntryEditEvent.Created, string.Empty);
				await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), archived);

				await session.AuditLogger.AuditLogAsync($"created {_entryLinkFactory.CreateEntryLink(ev)}");

				await _followedArtistNotifier.SendNotificationsAsync(session, ev, ev.Artists.Where(a => a?.Artist != null).Select(a => a.Artist), PermissionContext.LoggedUser);
			}
			else
			{
				ev = await session.LoadAsync(contract.Id);
				_permissionContext.VerifyEntryEdit(ev);

				var diff = new ReleaseEventDiff(DoSnapshot(ev, session));

				if (ev.Category != contract.Category)
					diff.Category.Set();

				if (!ev.Date.Equals(contract.Date) || !ev.EndDate.Equals(contract.EndDate))
					diff.Date.Set();

				if (ev.Description != contract.Description)
					diff.Description.Set();

				var inheritedLanguage = ev.Series == null || contract.CustomName ? contract.DefaultNameLanguage : ev.Series.TranslatedName.DefaultLanguage;
				if (ev.TranslatedName.DefaultLanguage != inheritedLanguage)
				{
					diff.OriginalName.Set();
				}

				var namesChanged = new UpdateEventNamesQuery().UpdateNames(
					session,
					ev,
					seriesLink: contract.Series,
					customName: contract.CustomName,
					seriesNumber: contract.SeriesNumber,
					seriesSuffix: contract.SeriesSuffix,
					nameContracts: contract.Names
				);

				if (namesChanged)
				{
					diff.Names.Set();
				}

				if (!ev.Series.NullSafeIdEquals(contract.Series))
				{
					diff.Series.Set();
				}

				if (ev.SeriesNumber != contract.SeriesNumber)
					diff.SeriesNumber.Set();

				if (ev.SeriesSuffix != contract.SeriesSuffix)
					diff.SeriesSuffix.Set();

				if (!ev.SongList.NullSafeIdEquals(contract.SongList))
				{
					diff.SongList.Set();
				}

				if (ev.Status != contract.Status)
					diff.Status.Set();

				if (!ev.Venue.NullSafeIdEquals(contract.Venue))
				{
					diff.Venue.Set();
				}

				if (!string.Equals(ev.VenueName, contract.VenueName))
				{
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

				if (weblinksDiff.Changed)
				{
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

				if (pictureData != null)
				{
					diff.MainPicture.Set();
					SaveImage(ev, pictureData);
				}

				await session.UpdateAsync(ev);

				var archived = Archive(session, ev, diff, EntryEditEvent.Updated, string.Empty);
				await AddEntryEditedEntryAsync(session.OfType<ActivityEntry>(), archived);

				var logStr = $"updated properties for {_entryLinkFactory.CreateEntryLink(ev)} ({diff.ChangedFieldsString})";
				await session.AuditLogger.AuditLogAsync(logStr);

				var newSongCutoff = TimeSpan.FromHours(1);
				if (artistDiff.Added.Any() && ev.CreateDate >= DateTime.Now - newSongCutoff)
				{
					var addedArtists = artistDiff.Added.Where(a => a.Artist != null).Select(a => a.Artist).Distinct().ToArray();

					if (addedArtists.Any())
					{
						await _followedArtistNotifier.SendNotificationsAsync(session, ev, addedArtists, PermissionContext.LoggedUser);
					}
				}
			}

			return new ReleaseEventContract(ev, LanguagePreference);
		});
	}

	[return: NotNullIfNotNull(nameof(pictureData))]
	private PictureDataContract? SaveImage(IEntryImageInformation entry, EntryPictureFileContract? pictureData)
	{
		if (pictureData == null) return null;

		var parsed = ImageHelper.GetOriginal(pictureData.UploadedFile, pictureData.ContentLength, pictureData.Mime);

		pictureData.Id = entry.Id;
		pictureData.EntryType = entry.EntryType;
		var thumbGenerator = new ImageThumbGenerator(_imagePersister);
		thumbGenerator.GenerateThumbsAndMoveImage(pictureData.UploadedFile, pictureData, ReleaseEventSeries.ImageSizes, originalSize: Constants.RestrictedImageOriginalSize);
		return parsed;
	}

	private void SaveImage(ReleaseEventSeries series, EntryPictureFileContract pictureData)
	{
		var parsed = SaveImage((IEntryImageInformation)series, pictureData);
		series.PictureMime = parsed.Mime;
	}

	private void SaveImage(ReleaseEvent ev, EntryPictureFileContract pictureData)
	{
		var parsed = SaveImage((IEntryImageInformation)ev, pictureData);
		ev.PictureMime = parsed.Mime;
	}

	public int UpdateSeries(ReleaseEventSeriesForEditForApiContract contract, EntryPictureFileContract? pictureData)
	{
		ParamIs.NotNull(() => contract);

		PermissionContext.VerifyManageDatabase();

		return HandleTransaction(session =>
		{
			ReleaseEventSeries series;

			if (contract.Id == 0)
			{
				series = new ReleaseEventSeries(contract.DefaultNameLanguage, contract.Names, contract.Description)
				{
					Category = contract.Category,
					Status = contract.Status
				};
				session.Save(series);

				var diff = new ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields.OriginalName | ReleaseEventSeriesEditableFields.Names);

				diff.Description.Set(!string.IsNullOrEmpty(contract.Description));

				var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

				if (weblinksDiff.Changed)
				{
					diff.WebLinks.Set();
					session.OfType<ReleaseEventWebLink>().Sync(weblinksDiff);
				}

				if (pictureData != null)
				{
					diff.Picture.Set();
					SaveImage(series, pictureData);
					session.Update(series);
				}

				session.Update(series);

				Archive(session, series, diff, EntryEditEvent.Created, string.Empty);

				AuditLog($"created {_entryLinkFactory.CreateEntryLink(series)}", session);
			}
			else
			{
				series = session.Load<ReleaseEventSeries>(contract.Id);
				_permissionContext.VerifyEntryEdit(series);
				var diff = new ReleaseEventSeriesDiff(DoSnapshot(series, session));

				if (series.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage)
				{
					series.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var nameDiff = series.Names.Sync(contract.Names, series);
				session.Sync(nameDiff);

				if (nameDiff.Changed)
				{
					diff.Names.Set();
				}


				if (series.Category != contract.Category)
				{
					diff.Category.Set();
					series.Category = contract.Category;
				}

				if (series.Description != contract.Description)
				{
					diff.Description.Set();
					series.Description = contract.Description;
				}

				if (series.Status != contract.Status)
				{
					diff.Status.Set();
					series.Status = contract.Status;
				}

				if (pictureData != null)
				{
					diff.Picture.Set();
					SaveImage(series, pictureData);
				}

				var weblinksDiff = WebLink.Sync(series.WebLinks, contract.WebLinks, series);

				if (weblinksDiff.Changed)
				{
					diff.WebLinks.Set();
					session.Sync(weblinksDiff);
				}

				session.Update(series);

				if (diff.Names.IsChanged || diff.OriginalName.IsChanged)
				{
					var eventNamesQuery = new UpdateEventNamesQuery();
					foreach (var ev in series.Events.Where(e => !e.CustomName))
					{
						eventNamesQuery.UpdateNames(session, ev, series, ev.CustomName, ev.SeriesNumber, ev.SeriesSuffix, ev.Names);
						ev.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
						session.Update(ev);
					}
				}

				Archive(session, series, diff, EntryEditEvent.Updated, string.Empty);

				AuditLog($"updated {_entryLinkFactory.CreateEntryLink(series)}", session);
			}

			return series.Id;
		});
	}
#nullable disable

	public AlbumForApiContract[] GetAlbums(
		int eventId,
		AlbumOptionalFields fields = AlbumOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		return _repository.HandleQuery(ctx =>
		{
			var ev = ctx.Load(eventId);
			return ev.Albums.Select(a => new AlbumForApiContract(a, null, lang, PermissionContext, _imageUrlFactory, fields, SongOptionalFields.None)).ToArray();
		});
	}

	public SongForApiContract[] GetPublishedSongs(
		int eventId,
		SongOptionalFields fields = SongOptionalFields.None,
		ContentLanguagePreference lang = ContentLanguagePreference.Default
	)
	{
		return _repository.HandleQuery(ctx =>
		{
			var ev = ctx.Load(eventId);
			return ev.Songs.Select(a => new SongForApiContract(a, lang, PermissionContext, fields)).ToArray();
		});
	}

	public string[] GetNames(
		string query = "",
		int maxResults = 10)
	{
		return _repository.HandleQuery(ctx =>
		{
			return ctx.Query<EventName>()
				.Where(n => !n.Entry.Deleted && n.Value.Contains(query))
				.OrderBy(n => n.Value)
				.Take(maxResults)
				.Select(r => r.Value)
				.ToArray();
		});
	}

#nullable enable
	public ReleaseEventSeriesDetailsForApiContract GetSeriesDetails(int id)
	{
		return HandleQuery(session => new ReleaseEventSeriesDetailsForApiContract(
			session.Load<ReleaseEventSeries>(id),
			LanguagePreference,
			PermissionContext,
			_imageUrlFactory
		));
	}

	public EntryWithArchivedVersionsForApiContract<ReleaseEventForApiContract> GetReleaseEventWithArchivedVersionsForApi(int id)
	{
		return HandleQuery(session =>
		{
			var ev = session.Load<ReleaseEvent>(id);
			return EntryWithArchivedVersionsForApiContract.Create(
				entry: new ReleaseEventForApiContract(ev, LanguagePreference, PermissionContext, fields: ReleaseEventOptionalFields.None, thumbPersister: null),
				versions: ev.ArchivedVersionsManager.Versions
					.Select(a => ArchivedObjectVersionForApiContract.FromReleaseEvent(a, _userIconFactory))
					.ToArray()
			);
		});
	}

	public EntryWithArchivedVersionsForApiContract<ReleaseEventSeriesForApiContract> GetReleaseEventSeriesWithArchivedVersionsForApi(int id)
	{
		return HandleQuery(session =>
		{
			var series = session.Load<ReleaseEventSeries>(id);
			return EntryWithArchivedVersionsForApiContract.Create(
				entry: new ReleaseEventSeriesForApiContract(series, LanguagePreference, PermissionContext, fields: ReleaseEventSeriesOptionalFields.None, thumbPersister: null),
				versions: series.ArchivedVersionsManager.Versions
					.Select(v => ArchivedObjectVersionForApiContract.FromReleaseEventSeries(v, _userIconFactory))
					.ToArray()
			);
		});
	}

	public ReleaseEventSeriesForEditForApiContract GetReleaseEventSeriesForEditForApi(int id)
	{
		return HandleQuery(session => new ReleaseEventSeriesForEditForApiContract(
			series: session.Load<ReleaseEventSeries>(id),
			languagePreference: LanguagePreference,
			thumbPersister: _imageUrlFactory
		));
	}

	public ReleaseEventSeriesWithEventsForApiContract[] GetReleaseEventsBySeries()
	{
		return HandleQuery(session =>
		{
			var allEvents = session.Query<ReleaseEvent>()
				.Where(e => !e.Deleted)
				.ToArray();
			var series = session.Query<ReleaseEventSeries>()
				.Where(e => !e.Deleted)
				.OrderByName(LanguagePreference)
				.ToArray();

			var seriesContracts = series.Select(s => new ReleaseEventSeriesWithEventsForApiContract(
				series: s,
				events: allEvents.Where(e => s.Equals(e.Series)),
				languagePreference: PermissionContext.LanguagePreference,
				PermissionContext,
				thumbPersister: _imageUrlFactory
			));
			var ungrouped = allEvents
				.Where(e => e.Series == null)
				.OrderBy(e => e.TranslatedName[LanguagePreference]);

			return seriesContracts.Append(new ReleaseEventSeriesWithEventsForApiContract
			{
				Name = string.Empty,
				Events = ungrouped
					.Select(e => new ReleaseEventForApiContract(
						rel: e,
						languagePreference: LanguagePreference,
						PermissionContext,
						fields: ReleaseEventOptionalFields.None,
						thumbPersister: null
					))
					.ToArray(),
			}).ToArray();
		});
	}
#nullable disable
}
