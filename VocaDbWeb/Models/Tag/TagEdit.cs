using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Tag {

	[PropertyModelBinder]
	public class TagEdit {

		public TagEdit() {}

		public TagEdit(TagForEditContract contract, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => contract);

			AliasedTo = contract.AliasedTo;
			CategoryName = contract.CategoryName;
			Description = contract.Description;
			EnglishName = contract.EnglishName;
			Name = contract.Name;
			Parent = contract.Parent;
			Status = contract.Status;
			Thumb = contract.Thumb;

			CopyNonEditableProperties(contract, permissionContext);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[FromJson]
		public TagBaseContract AliasedTo { get; set; }

		[Display(Name = "Category")]
		[StringLength(30)]
		public string CategoryName { get; set; }

		[Display(Name = "Description")]
		[StringLength(1000)]
		public string Description { get; set; }

		public bool Draft => Status == EntryStatus.Draft;

		[Display(Name = "English name")]
		[StringLength(100)]
		public string EnglishName { get; set; }

		public int Id { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		[FromJson]
		public TagBaseContract Parent { get; set; }

		public EntryStatus Status { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string UpdateNotes { get; set; }

		public string UrlSlug { get; set; }

		public void CopyNonEditableProperties(TagForEditContract contract, IUserPermissionContext permissionContext) {

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
			Id = contract.Id;
			IsEmpty = contract.IsEmpty;
			Thumb = contract.Thumb;
			UrlSlug = contract.UrlSlug;

		}

		public TagForEditContract ToContract() {

			return new TagForEditContract {
				Id = this.Id,
				Name = this.Name,
				AliasedTo = this.AliasedTo,
				CategoryName = this.CategoryName ?? string.Empty,
				Description = this.Description ?? string.Empty,
				EnglishName = this.EnglishName,
				Parent = this.Parent,
				Status = this.Status,
				UpdateNotes = this.UpdateNotes ?? string.Empty
			};

		}

	}
}