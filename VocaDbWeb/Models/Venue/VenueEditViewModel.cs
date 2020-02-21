using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Venues;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Globalization;

namespace VocaDb.Web.Models.Venue {

	public class VenueEditViewModel {

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		public LocalizedStringWithIdContract[] Names { get; set; }

		public EntryStatus Status { get; set; }

		public WebLinkContract[] WebLinks { get; set; }

		public VenueEditViewModel() { }

		public VenueEditViewModel(VenueForEditContract contract, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => contract);

			DefaultNameLanguage = contract.DefaultNameLanguage;
			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;
			Names = contract.Names;
			Status = contract.Status;
			WebLinks = contract.WebLinks;

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();

		}

		public VenueForEditContract ToContract() => new VenueForEditContract {
			DefaultNameLanguage = DefaultNameLanguage,
			Description = Description ?? string.Empty,
			Id = Id,
			Name = Name,
			Names = Names,
			Status = Status,
			WebLinks = WebLinks
		};

	}

}