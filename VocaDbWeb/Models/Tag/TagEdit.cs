using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Web.Models.Tag {

	public class TagEdit {

		public TagEdit() {}

		public TagEdit(TagForEditContract contract, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => contract);

			AliasedTo = contract.AliasedTo?.Name;
			CategoryName = contract.CategoryName;
			Description = contract.Description;
			Name = contract.Name;
			Parent = contract.Parent?.Name;
			Status = contract.Status;
			Thumb = contract.Thumb;

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
			CopyNonEditableProperties(contract);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[Display(Name = "Aliased to")]
		[StringLength(30)]
		public string AliasedTo { get; set; }

		[Display(Name = "Category")]
		[StringLength(30)]
		public string CategoryName { get; set; }

		[Display(Name = "Description")]
		[StringLength(1000)]
		public string Description { get; set; }

		public bool Draft => Status == EntryStatus.Draft;

		public int Id { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		public string Parent { get; set; }

		public EntryStatus Status { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string UpdateNotes { get; set; }

		public string UrlSlug { get; set; }

		public void CopyNonEditableProperties(TagForEditContract contract) {

			Id = contract.Id;
			IsEmpty = contract.IsEmpty;
			Thumb = contract.Thumb;
			UrlSlug = contract.UrlSlug;

		}

		public TagForEditContract ToContract() {

			return new TagForEditContract {
				Id = this.Id,
				Name = this.Name,
				AliasedTo = new TagBaseContract { Name = this.AliasedTo ?? string.Empty },
				CategoryName = this.CategoryName ?? string.Empty,
				Description = this.Description ?? string.Empty,
				Parent = new TagBaseContract { Name = this.Parent ?? string.Empty },
				Status = this.Status,
				UpdateNotes = this.UpdateNotes ?? string.Empty
			};

		}

	}
}