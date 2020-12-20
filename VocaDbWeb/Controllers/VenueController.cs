#nullable disable

using System.Linq;
using System.Web.Mvc;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Venues;
using VocaDb.Model.Service.Translations;
using VocaDb.Web.Models.Shared;
using VocaDb.Web.Models.Venue;

namespace VocaDb.Web.Controllers
{
	public class VenueController : ControllerBase
	{
		private readonly IEnumTranslations _enumTranslations;
		private readonly VenueQueries _queries;

		public VenueController(VenueQueries queries, IEnumTranslations enumTranslations)
		{
			_queries = queries;
			_enumTranslations = enumTranslations;
		}

		public ActionResult Details(int id = InvalidId)
		{
			var venue = _queries.GetDetails(id);

			PageProperties.Title = venue.Name;
			PageProperties.Subtitle = ViewRes.Venue.DetailsStrings.Venue;

			return View(venue);
		}

		[Authorize]
		public ActionResult Edit(int? id)
		{
			if (id.HasValue)
			{
				CheckConcurrentEdit(EntryType.Venue, id.Value);
			}

			var contract = id.HasValue ? _queries.GetForEdit(id.Value) : new VenueForEditContract();
			return View(new VenueEditViewModel(contract, PermissionContext));
		}

		[HttpPost]
		[Authorize]
		public ActionResult Edit(VenueEditViewModel model)
		{
			// Note: name is allowed to be whitespace, but not empty.
			if (model.Names == null || model.Names.All(n => string.IsNullOrEmpty(n?.Value)))
			{
				ModelState.AddModelError("Names", "Name cannot be empty");
			}

			if ((model.Coordinates != null) && !OptionalGeoPoint.IsValid(model.Coordinates.Latitude, model.Coordinates.Longitude))
			{
				ModelState.AddModelError("Coordinates", "Invalid coordinates");
			}

			if (!ModelState.IsValid)
			{
				model.AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(PermissionContext).ToArray();
				return View(model);
			}

			var id = _queries.Update(model.ToContract());

			return RedirectToAction("Details", new { id });
		}

		public ActionResult Restore(int id)
		{
			_queries.Restore(id);

			return RedirectToAction("Edit", new { id });
		}

		public ActionResult UpdateVersionVisibility(int archivedVersionId, bool hidden)
		{
			_queries.UpdateVersionVisibility<ArchivedVenueVersion>(archivedVersionId, hidden);

			return RedirectToAction("ViewVersion", new { id = archivedVersionId });
		}

		public ActionResult Versions(int id = InvalidId)
		{
			var contract = _queries.GetWithArchivedVersions(id);

			return View(new Versions(contract, _enumTranslations));
		}

		public ActionResult ViewVersion(int id, int? ComparedVersionId)
		{
			var contract = _queries.GetVersionDetails(id, ComparedVersionId ?? 0);

			return View(new ViewVersion<ArchivedVenueVersionDetailsContract>(contract, _enumTranslations, contract.ComparedVersionId));
		}
	}
}