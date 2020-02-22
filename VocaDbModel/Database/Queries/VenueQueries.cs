using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.ExtLinks;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries {

	public class VenueQueries : QueriesBase<IVenueRepository, Venue> {

		private readonly IEntryLinkFactory entryLinkFactory;

		public VenueQueries(IVenueRepository venueRepository, IEntryLinkFactory entryLinkFactory, IUserPermissionContext permissionContext)
			: base(venueRepository, permissionContext) {

			this.entryLinkFactory = entryLinkFactory;

		}
		
		private ArchivedVenueVersion Archive(IDatabaseContext<Venue> ctx, Venue venue, VenueDiff diff, EntryEditEvent reason, string notes) {

			var agentLoginData = ctx.OfType<User>().CreateAgentLoginData(permissionContext);
			var archived = ArchivedVenueVersion.Create(venue, diff, agentLoginData, reason, notes);
			ctx.Save(archived);
			return archived;

		}

		public VenueDetailsContract GetDetails(int id) {

			return HandleQuery(ctx => new VenueDetailsContract(ctx.Load(id), LanguagePreference));

		}

		public VenueForEditContract GetForEdit(int id) {

			return HandleQuery(ctx => new VenueForEditContract(ctx.Load(id), LanguagePreference));

		}

		public ArchivedVenueVersionDetailsContract GetVersionDetails(int id, int comparedVersionId) {

			return HandleQuery(session =>
				new ArchivedVenueVersionDetailsContract(session.Load<ArchivedVenueVersion>(id),
					comparedVersionId != 0 ? session.Load<ArchivedVenueVersion>(comparedVersionId) : null,
					PermissionContext.LanguagePreference));

		}

		public VenueWithArchivedVersionsContract GetWithArchivedVersions(int id) {

			return HandleQuery(ctx => new VenueWithArchivedVersionsContract(ctx.Load(id), LanguagePreference));

		}

		public int Update(VenueForEditContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyManageDatabase();

			return HandleTransaction(ctx => {

				Venue venue;

				if (contract.Id == 0) {

					venue = new Venue(contract.DefaultNameLanguage, contract.Names, contract.Description) {
						Status = contract.Status
					};
					ctx.Save(venue);

					var diff = new VenueDiff(VenueEditableFields.OriginalName | VenueEditableFields.Names);

					diff.Description.Set(!string.IsNullOrEmpty(contract.Description));

					var weblinksDiff = WebLink.Sync(venue.WebLinks, contract.WebLinks, venue);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						ctx.OfType<VenueWebLink>().Sync(weblinksDiff);
					}

					ctx.Update(venue);

					Archive(ctx, venue, diff, EntryEditEvent.Created, string.Empty);

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

					if (venue.Description != contract.Description) {
						diff.Description.Set();
						venue.Description = contract.Description;
					}

					if (venue.Status != contract.Status) {
						diff.Status.Set();
						venue.Status = contract.Status;
					}

					var weblinksDiff = WebLink.Sync(venue.WebLinks, contract.WebLinks, venue);

					if (weblinksDiff.Changed) {
						diff.WebLinks.Set();
						ctx.Sync(weblinksDiff);
					}

					ctx.Update(venue);

					Archive(ctx, venue, diff, EntryEditEvent.Updated, string.Empty);

					AuditLog(string.Format("updated {0}", entryLinkFactory.CreateEntryLink(venue)), ctx);

				}

				return venue.Id;

			});

		}

	}

}
