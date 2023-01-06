#nullable disable

using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.DataContracts.Versioning;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Search.Venues;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries;

public class VenueQueries : QueriesBase<IVenueRepository, Venue>
{
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly IEnumTranslations _enumTranslations;
	private readonly IUserIconFactory _userIconFactory;
	private readonly IDiscordWebhookNotifier _discordWebhookNotifier;

	public VenueQueries(
		IVenueRepository venueRepository,
		IEntryLinkFactory entryLinkFactory,
		IUserPermissionContext permissionContext,
		IEnumTranslations enumTranslations,
		IUserIconFactory userIconFactory,
		IDiscordWebhookNotifier discordWebhookNotifier
	)
		: base(venueRepository, permissionContext)
	{
		_entryLinkFactory = entryLinkFactory;
		_enumTranslations = enumTranslations;
		_userIconFactory = userIconFactory;
		_discordWebhookNotifier = discordWebhookNotifier;
	}

	private ArchivedVenueVersion Archive(IDatabaseContext<Venue> ctx, Venue venue, VenueDiff diff, EntryEditEvent reason, string notes)
	{
		var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(_permissionContext);
		var archived = ArchivedVenueVersion.Create(venue, diff, agentLoginData, reason, notes);
		ctx.Save(archived);
		return archived;
	}

#nullable enable
	public Task<(bool created, int reportId)> CreateReport(int venueId, VenueReportType reportType, string hostname, string notes, int? versionNumber)
	{
		ParamIs.NotNull(() => hostname);
		ParamIs.NotNull(() => notes);

		return HandleTransactionAsync(ctx =>
		{
			return new Service.Queries.EntryReportQueries().CreateReport(
				ctx,
				PermissionContext,
				_entryLinkFactory,
				(song, reporter, notesTruncated) => new VenueReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
				() => reportType != VenueReportType.Other ? _enumTranslations.Translation(reportType) : null,
				venueId,
				reportType,
				hostname,
				notes,
				_discordWebhookNotifier,
				VenueReport.ReportTypesWithRequiredNotes
			);
		});
	}
#nullable disable

	private void CreateTrashedEntry(IDatabaseContext ctx, Venue venue, string notes)
	{
		var archived = new ArchivedVenueContract(venue, new VenueDiff(true));
		var data = XmlHelper.SerializeToXml(archived);
		var trashed = new TrashedEntry(venue, data, GetLoggedUser(ctx), notes);

		ctx.Save(trashed);
	}

	public void Delete(int id, string notes)
	{
		_permissionContext.VerifyManageDatabase();

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load(id);

			PermissionContext.VerifyEntryDelete(entry);

			entry.Deleted = true;
			ctx.Update(entry);

			Archive(ctx, entry, new VenueDiff(false), EntryEditEvent.Deleted, notes);

			ctx.AuditLogger.AuditLog($"deleted {entry}");
		});
	}

	public PartialFindResult<TResult> Find<TResult>(Func<Venue, TResult> fac, VenueQueryParams queryParams)
	{
		return HandleQuery(ctx =>
		{
			var q = ctx.Query<Venue>()
				.WhereNotDeleted()
				.WhereHasName(queryParams.TextQuery)
				.WhereInCircle(queryParams.Coordinates, queryParams.Radius, queryParams.DistanceUnit)
				.Paged(queryParams.Paging);

			var entries = q
				.OrderBy(queryParams.SortRule, PermissionContext.LanguagePreference, queryParams.Coordinates, queryParams.DistanceUnit)
				.ToArray()
				.Select(fac)
				.ToArray();

			var count = queryParams.Paging.GetTotalCount ? q.Count() : 0;

			return PartialFindResult.Create(entries, count);
		});
	}

#nullable enable
	public VenueForApiContract GetDetails(int id)
	{
		return HandleQuery(ctx => new VenueForApiContract(
			venue: ctx.Load(id),
			languagePreference: LanguagePreference,
			fields:
				VenueOptionalFields.AdditionalNames |
				VenueOptionalFields.Description |
				VenueOptionalFields.Events |
				VenueOptionalFields.Names |
				VenueOptionalFields.WebLinks
		));
	}
#nullable disable

	public VenueForEditForApiContract GetForEdit(int id)
	{
		return HandleQuery(ctx => new VenueForEditForApiContract(ctx.Load(id), LanguagePreference));
	}

	[Obsolete]
	public ArchivedVenueVersionDetailsContract GetVersionDetails(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedVenueVersionDetailsContract(session.Load<ArchivedVenueVersion>(id),
				comparedVersionId != 0 ? session.Load<ArchivedVenueVersion>(comparedVersionId) : null,
				PermissionContext, _userIconFactory);

			if (contract.Hidden)
			{
				PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
			}

			return contract;
		});
	}

#nullable enable
	public ArchivedVenueVersionDetailsForApiContract GetVersionDetailsForApi(int id, int comparedVersionId)
	{
		return HandleQuery(session =>
		{
			var contract = new ArchivedVenueVersionDetailsForApiContract(
				archived: session.Load<ArchivedVenueVersion>(id),
				comparedVersion: comparedVersionId != 0 ? session.Load<ArchivedVenueVersion>(comparedVersionId) : null,
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
	public VenueWithArchivedVersionsContract GetWithArchivedVersions(int id)
	{
		return HandleQuery(ctx => new VenueWithArchivedVersionsContract(ctx.Load(id), LanguagePreference, _userIconFactory));
	}

#nullable enable
	public EntryWithArchivedVersionsForApiContract<VenueForApiContract> GetVenueWithArchivedVersionsForApi(int id)
	{
		return HandleQuery(ctx =>
		{
			var venue = ctx.Load(id);
			return EntryWithArchivedVersionsForApiContract.Create(
				entry: new VenueForApiContract(venue, LanguagePreference, fields: VenueOptionalFields.None),
				versions: venue.ArchivedVersionsManager.Versions
					.Select(a => ArchivedObjectVersionForApiContract.FromVenue(a, _userIconFactory))
					.ToArray()
			);
		});
	}
#nullable disable

	public void MoveToTrash(int id, string notes)
	{
		PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

		_repository.HandleTransaction(ctx =>
		{
			var entry = ctx.Load(id);

			PermissionContext.VerifyEntryDelete(entry);

			ctx.AuditLogger.SysLog($"moving {entry} to trash");

			CreateTrashedEntry(ctx, entry, notes);

			var allEvents = entry.AllEvents.ToArray();
			foreach (var ev in allEvents)
			{
				ev.SetVenue(null);
			}

			var ctxActivity = ctx.OfType<VenueActivityEntry>();
			var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == id).ToArray();

			foreach (var activityEntry in activityEntries)
				ctxActivity.Delete(activityEntry);

			ctx.Delete(entry);

			ctx.AuditLogger.AuditLog($"moved {entry} to trash");
		});
	}

	public void Restore(int id)
	{
		PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

		HandleTransaction(ctx =>
		{
			var venue = ctx.Load<Venue>(id);

			venue.Deleted = false;

			ctx.Update(venue);

			Archive(ctx, venue, new VenueDiff(false), EntryEditEvent.Restored, string.Empty);

			ctx.AuditLogger.AuditLog($"restored {venue}");
		});
	}

#nullable enable
	public int Update(VenueForEditForApiContract contract)
	{
		ParamIs.NotNull(() => contract);

		PermissionContext.VerifyManageDatabase();

		return HandleTransaction(ctx =>
		{
			Venue venue;

			if (contract.Id == 0)
			{
				venue = new Venue(contract.DefaultNameLanguage, contract.Names, contract.Description)
				{
					Address = contract.Address,
					AddressCountryCode = contract.AddressCountryCode,
					Coordinates = (contract.Coordinates != null) ? new OptionalGeoPoint(contract.Coordinates) : new OptionalGeoPoint(),
					Status = contract.Status
				};
				ctx.Save(venue);

				var diff = new VenueDiff(VenueEditableFields.OriginalName | VenueEditableFields.Names);

				diff.Address.Set(!string.IsNullOrEmpty(contract.Address));
				diff.AddressCountryCode.Set(!string.IsNullOrEmpty(contract.AddressCountryCode));
				diff.Description.Set(!string.IsNullOrEmpty(contract.Description));

				if (contract.Coordinates != null)
				{
					diff.Coordinates.Set();
				}

				var webLinkDiff = venue.WebLinks.Sync(contract.WebLinks, venue);
				ctx.OfType<VenueWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks.Set();

				ctx.Update(venue);

				var archived = Archive(ctx, venue, diff, EntryEditEvent.Created, string.Empty);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), venue, EntryEditEvent.Created, archived);

				AuditLog($"created {_entryLinkFactory.CreateEntryLink(venue)}", ctx);
			}
			else
			{
				venue = ctx.Load<Venue>(contract.Id);
				_permissionContext.VerifyEntryEdit(venue);
				var diff = new VenueDiff(DoSnapshot(venue, ctx));

				if (venue.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage)
				{
					venue.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
					diff.OriginalName.Set();
				}

				var nameDiff = venue.Names.Sync(contract.Names, venue);
				ctx.Sync(nameDiff);

				if (nameDiff.Changed)
				{
					diff.Names.Set();
				}

				if (venue.Address != contract.Address)
				{
					diff.Address.Set();
					venue.Address = contract.Address;
				}

				if (venue.AddressCountryCode != contract.AddressCountryCode)
				{
					diff.AddressCountryCode.Set();
					venue.AddressCountryCode = contract.AddressCountryCode;
				}

				if (venue.Description != contract.Description)
				{
					diff.Description.Set();
					venue.Description = contract.Description;
				}

				if (!venue.Coordinates.Equals(contract.Coordinates))
				{
					diff.Coordinates.Set();
					venue.Coordinates = (contract.Coordinates != null) ? new OptionalGeoPoint(contract.Coordinates) : new OptionalGeoPoint();
				}

				if (venue.Status != contract.Status)
				{
					diff.Status.Set();
					venue.Status = contract.Status;
				}

				var webLinkDiff = venue.WebLinks.Sync(contract.WebLinks, venue);
				ctx.OfType<VenueWebLink>().Sync(webLinkDiff);

				if (webLinkDiff.Changed)
					diff.WebLinks.Set();

				ctx.Update(venue);

				var archived = Archive(ctx, venue, diff, EntryEditEvent.Updated, string.Empty);
				AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), venue, EntryEditEvent.Updated, archived);

				AuditLog($"updated {_entryLinkFactory.CreateEntryLink(venue)}", ctx);
			}

			return venue.Id;
		});
	}

	public VenueForApiContract GetOne(int id)
	{
		return HandleQuery(ctx => new VenueForApiContract(ctx.Load(id), LanguagePreference, VenueOptionalFields.None));
	}
#nullable disable
}
