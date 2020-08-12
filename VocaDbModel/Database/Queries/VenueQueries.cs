using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Search.Venues;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Database.Queries {

	public class VenueQueries : QueriesBase<IVenueRepository, Venue> {

		private readonly IEntryLinkFactory entryLinkFactory;
		private readonly IEnumTranslations enumTranslations;

		public VenueQueries(IVenueRepository venueRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext, IEnumTranslations enumTranslations)
			: base(venueRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;
			this.enumTranslations = enumTranslations;

		}
		
		private ArchivedVenueVersion Archive(IDatabaseContext<Venue> ctx, Venue venue, VenueDiff diff, EntryEditEvent reason, string notes) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = ArchivedVenueVersion.Create(venue, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		public (bool created, int reportId) CreateReport(int venueId, VenueReportType reportType, string hostname, string notes, int? versionNumber) {

			ParamIs.NotNull(() => hostname);
			ParamIs.NotNull(() => notes);

			return HandleTransaction(ctx => {
				return new Model.Service.Queries.EntryReportQueries().CreateReport(ctx, PermissionContext,
					entryLinkFactory,
					(song, reporter, notesTruncated) => new VenueReport(song, reportType, reporter, hostname, notesTruncated, versionNumber),
					() => reportType != VenueReportType.Other ? enumTranslations.Translation(reportType) : null,
					venueId, reportType, hostname, notes);
			});

		}
		
		private void CreateTrashedEntry(IDatabaseContext ctx, Venue venue, string notes) {

			var archived = new ArchivedVenueContract(venue, new VenueDiff(true));
			var data = XmlHelper.SerializeToXml(archived);
			var trashed = new TrashedEntry(venue, data, GetLoggedUser(ctx), notes);

			ctx.Save(trashed);

		}

		public void Delete(int id, string notes) {

			permissionContext.VerifyManageDatabase();

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(id);

				PermissionContext.VerifyEntryDelete(entry);

				entry.Deleted = true;
				ctx.Update(entry);

				Archive(ctx, entry, new VenueDiff(false), EntryEditEvent.Deleted, notes);

				ctx.AuditLogger.AuditLog(string.Format("deleted {0}", entry));

			});

		}
		
		public PartialFindResult<TResult> Find<TResult>(Func<Venue, TResult> fac, VenueQueryParams queryParams) {

			return HandleQuery(ctx => {

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

		public VenueForApiContract GetDetails(int id) {

			return HandleQuery(ctx => new VenueForApiContract(
				ctx.Load(id),
				LanguagePreference,
				VenueOptionalFields.AdditionalNames | VenueOptionalFields.Description | VenueOptionalFields.Events | VenueOptionalFields.Names | VenueOptionalFields.WebLinks));

		}

		public VenueForEditContract GetForEdit(int id) {

			return HandleQuery(ctx => new VenueForEditContract(ctx.Load(id), LanguagePreference));

		}

		public ArchivedVenueVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session => {

				var contract = new ArchivedVenueVersionDetailsContract(session.Load<ArchivedVenueVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedVenueVersion>(comparedVersionId) : null,
					PermissionContext);

				if (contract.Hidden) {
					PermissionContext.VerifyPermission(PermissionToken.ViewHiddenRevisions);
				}

				return contract;

			});

		}

		public VenueWithArchivedVersionsContract GetWithArchivedVersions(int id) {

			return HandleQuery(ctx => new VenueWithArchivedVersionsContract(ctx.Load(id), LanguagePreference));

		}
		
		public void MoveToTrash(int id, string notes) {

			PermissionContext.VerifyPermission(PermissionToken.MoveToTrash);

			repository.HandleTransaction(ctx => {

				var entry = ctx.Load(id);

				PermissionContext.VerifyEntryDelete(entry);

				ctx.AuditLogger.SysLog(string.Format("moving {0} to trash", entry));

				CreateTrashedEntry(ctx, entry, notes);
				
				var allEvents = entry.AllEvents.ToArray();
				foreach (var ev in allEvents) {
					ev.SetVenue(null);
				}

				var ctxActivity = ctx.OfType<VenueActivityEntry>();
				var activityEntries = ctxActivity.Query().Where(a => a.Entry.Id == id).ToArray();

				foreach (var activityEntry in activityEntries)
					ctxActivity.Delete(activityEntry);

				ctx.Delete(entry);

				ctx.AuditLogger.AuditLog(string.Format("moved {0} to trash", entry));

			});

		}
		
		public void Restore(int id) {

			PermissionContext.VerifyPermission(PermissionToken.DeleteEntries);

			HandleTransaction(ctx => {

				var venue = ctx.Load<Venue>(id);

				venue.Deleted = false;

				ctx.Update(venue);

				Archive(ctx, venue, new VenueDiff(false), EntryEditEvent.Restored, string.Empty);

				ctx.AuditLogger.AuditLog(string.Format("restored {0}", venue));

			});

		}

		public int Update(VenueForEditContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return HandleTransaction(ctx => {

				Venue venue;

				if (contract.Id == 0) {

					venue = new Venue(contract.DefaultNameLanguage, contract.Names, contract.Description) {
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

					if (contract.Coordinates != null) {
						diff.Coordinates.Set();
					}

					var webLinkDiff = venue.WebLinks.Sync(contract.WebLinks, venue);
					ctx.OfType<VenueWebLink>().Sync(webLinkDiff);

					if (webLinkDiff.Changed)
						diff.WebLinks.Set();

					ctx.Update(venue);

					var archived = Archive(ctx, venue, diff, EntryEditEvent.Created, string.Empty);
					AddEntryEditedEntry(ctx.OfType<ActivityEntry>(), venue, EntryEditEvent.Created, archived);

					AuditLog(string.Format("created {0}", entryLinkFactory.CreateEntryLink(venue)), ctx);

				} else {
					
					venue = ctx.Load<Venue>(contract.Id);
					permissionContext.VerifyEntryEdit(venue);
					var diff = new VenueDiff(DoSnapshot(venue, ctx));

					if (venue.TranslatedName.DefaultLanguage != contract.DefaultNameLanguage) {
						venue.TranslatedName.DefaultLanguage = contract.DefaultNameLanguage;
						diff.OriginalName.Set();
					}

					var nameDiff = venue.Names.Sync(contract.Names, venue);
					ctx.Sync(nameDiff);

					if (nameDiff.Changed) {
						diff.Names.Set();
					}

					if (venue.Address != contract.Address) {
						diff.Address.Set();
						venue.Address = contract.Address;
					}

					if (venue.AddressCountryCode != contract.AddressCountryCode) {
						diff.AddressCountryCode.Set();
						venue.AddressCountryCode = contract.AddressCountryCode;
					}

					if (venue.Description != contract.Description) {
						diff.Description.Set();
						venue.Description = contract.Description;
					}

					if (!venue.Coordinates.Equals(contract.Coordinates)) {
						diff.Coordinates.Set();
						venue.Coordinates = (contract.Coordinates != null) ? new OptionalGeoPoint(contract.Coordinates) : new OptionalGeoPoint();
					}

					if (venue.Status != contract.Status) {
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

					AuditLog(string.Format("updated {0}", entryLinkFactory.CreateEntryLink(venue)), ctx);

				}

				return venue.Id;

			});

		}

	}

}
