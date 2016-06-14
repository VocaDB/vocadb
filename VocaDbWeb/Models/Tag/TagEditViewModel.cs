using System.ComponentModel.DataAnnotations;
using System.Linq;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;

namespace VocaDb.Web.Models.Tag {

	[PropertyModelBinder]
	public class TagEditViewModel {

		public TagEditViewModel() {}

		public TagEditViewModel(TagForEditContract contract, IUserPermissionContext permissionContext) {

			ParamIs.NotNull(() => contract);

			AliasedTo = contract.AliasedTo;
			CategoryName = contract.CategoryName;
			DefaultNameLanguage = contract.DefaultNameLanguage;
			Description = contract.Description;
			HideFromSuggestions = contract.HideFromSuggestions;
			Name = contract.Name;
			Parent = contract.Parent;
			Status = contract.Status;
			Thumb = contract.Thumb;
			WebLinks = contract.WebLinks;

			CopyNonEditableProperties(contract, permissionContext);

		}

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		[FromJson]
		public TagBaseContract AliasedTo { get; set; }

		public bool CanDelete { get; set; }

		[Display(Name = "Category")]
		[StringLength(30)]
		public string CategoryName { get; set; }

		public string CurrentName { get; set; }

		public bool Deleted { get; set; }

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[Display(Name = "Description")]
		[FromJson]
		public EnglishTranslatedStringContract Description { get; set; }

		public bool Draft => Status == EntryStatus.Draft;

		public bool HideFromSuggestions { get; set; }

		public int Id { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		[FromJson]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[FromJson]
		public TagBaseContract Parent { get; set; }

		[FromJson]
		public TagBaseContract[] RelatedTags { get; set; }

		public EntryStatus Status { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string UpdateNotes { get; set; }

		public string UrlSlug { get; set; }

		[FromJson]
		public WebLinkContract[] WebLinks { get; set; }

		public void CheckModel() {

			if (Description == null)
				throw new InvalidFormException("Description was null");

			if (Names == null)
				throw new InvalidFormException("Names list was null");

			if (WebLinks == null)
				throw new InvalidFormException("WebLinks list was null");

		}

		public void CopyNonEditableProperties(TagForEditContract contract, IUserPermissionContext permissionContext) {

			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
			CanDelete = contract.CanDelete;
			CurrentName = contract.Name;
			Deleted = contract.Deleted;
			Id = contract.Id;
			IsEmpty = contract.IsEmpty;
			Thumb = contract.Thumb;
			UrlSlug = contract.UrlSlug;

		}

		public TagForEditContract ToContract() {

			return new TagForEditContract {
				Id = this.Id,
				Name = this.Name,
				Names = Names,
				AliasedTo = this.AliasedTo,
				CategoryName = this.CategoryName ?? string.Empty,
				DefaultNameLanguage = DefaultNameLanguage,
				Description = this.Description,
				HideFromSuggestions = HideFromSuggestions,
				Parent = this.Parent,
				RelatedTags = RelatedTags,
				Status = this.Status,
				UpdateNotes = this.UpdateNotes ?? string.Empty,
				WebLinks = WebLinks
			};

		}

	}
}