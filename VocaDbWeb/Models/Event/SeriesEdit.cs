using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Event {

	[PropertyModelBinder]
	public class SeriesEdit {

		public SeriesEdit() { 

		}

		public SeriesEdit(ReleaseEventSeriesForEditContract contract, IUserPermissionContext userContext) {

			ParamIs.NotNull(() => contract);

			Category = contract.Category;
			Contract = contract;
			DefaultNameLanguage = contract.DefaultNameLanguage;
			Description = contract.Description;
			Id = contract.Id;
			Name = contract.Name;
			Names = contract.Names;
			Status = contract.Status;
			WebLinks = contract.WebLinks;

			CopyNonEditableProperties(contract, userContext);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public EventCategory Category { get; set; }

		public ReleaseEventSeriesForEditContract Contract { get; set; }

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		public bool Deleted { get; set; }

		public string Description { get; set; }

		public int Id { get; set; }

		public string Name { get; set; }

		[FromJson]
		public LocalizedStringWithIdContract[] Names { get; set; }

		public EntryStatus Status { get; set; }

		[FromJson]
		public WebLinkContract[] WebLinks { get; set; }

		public void CopyNonEditableProperties(ReleaseEventSeriesForEditContract contract, IUserPermissionContext userContext) {

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(userContext).ToArray();

			if (contract != null) {
				Deleted = contract.Deleted;
			}

		}

		public ReleaseEventSeriesForEditContract ToContract() {

			return new ReleaseEventSeriesForEditContract { 
				Category = Category,
				DefaultNameLanguage = DefaultNameLanguage,
				Description = this.Description ?? string.Empty, 
				Id = this.Id,
				Name = this.Name,
				Names = this.Names.ToArray(),
				Status = Status,
				WebLinks = this.WebLinks
			};

		}

	}
}