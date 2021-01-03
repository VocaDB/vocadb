#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Web.Code;
using VocaDb.Web.Code.Exceptions;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Models.Tag
{
	public class TagEditViewModel
	{
		public TagEditViewModel() { }

		public TagEditViewModel(TagForEditContract contract, IUserPermissionContext permissionContext)
		{
			ParamIs.NotNull(() => contract);

			CategoryName = contract.CategoryName;
			DefaultNameLanguage = contract.DefaultNameLanguage;
			Description = contract.Description;
			HideFromSuggestions = contract.HideFromSuggestions;
			Name = contract.Name;
			Parent = contract.Parent;
			Status = contract.Status;
			Targets = contract.Targets;
			Thumb = contract.Thumb;
			WebLinks = contract.WebLinks;

			CopyNonEditableProperties(contract, permissionContext);
		}

		public Dictionary<TagTargetTypes, string> AllTagTargetTypes { get; set; }

		public EntryStatus[] AllowedEntryStatuses { get; set; }

		public bool CanDelete { get; set; }

		[Display(Name = "Category")]
		[StringLength(30)]
		public string CategoryName { get; set; }

		public string CurrentName { get; set; }

		public bool Deleted { get; set; }

		public ContentLanguageSelection DefaultNameLanguage { get; set; }

		[Display(Name = "Description")]
		[ModelBinder(BinderType = typeof(JsonModelBinder))]
		public EnglishTranslatedStringContract Description { get; set; }

		public bool Draft => Status == EntryStatus.Draft;

		public bool HideFromSuggestions { get; set; }

		public int Id { get; set; }

		public bool IsEmpty { get; set; }

		public string Name { get; set; }

		[ModelBinder(BinderType = typeof(JsonModelBinder))]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[ModelBinder(BinderType = typeof(JsonModelBinder))]
		public TagBaseContract Parent { get; set; }

		[ModelBinder(BinderType = typeof(JsonModelBinder))]
		public TagBaseContract[] RelatedTags { get; set; }

		public EntryStatus Status { get; set; }

		public TagTargetTypes Targets { get; set; }

		public EntryThumbContract Thumb { get; set; }

		public string UpdateNotes { get; set; }

		public string UrlSlug { get; set; }

		[ModelBinder(BinderType = typeof(JsonModelBinder))]
		public WebLinkContract[] WebLinks { get; set; }

		public void CheckModel()
		{
			if (Description == null)
				throw new InvalidFormException("Description was null");

			if (Names == null)
				throw new InvalidFormException("Names list was null");

			if (WebLinks == null)
				throw new InvalidFormException("WebLinks list was null");
		}

		public void CopyNonEditableProperties(TagForEditContract contract, IUserPermissionContext permissionContext)
		{
			AllowedEntryStatuses = EntryPermissionManager.AllowedEntryStatuses(permissionContext).ToArray();
			CanDelete = contract.CanDelete;
			CurrentName = contract.Name;
			Deleted = contract.Deleted;
			Id = contract.Id;
			IsEmpty = contract.IsEmpty;
			Thumb = contract.Thumb;
			UrlSlug = contract.UrlSlug;

			string GetTagTargetTypeName(TagTargetTypes t) => t switch
			{
				TagTargetTypes.Nothing => "Nothing",
				TagTargetTypes.All => "Anything",
				_ => string.Join(", ", EnumVal<EntryType>.Values.Where(e => e != EntryType.Undefined).Where(e => t.HasFlag((TagTargetTypes)e)).Select(e => Translate.EntryTypeNames[e])),
			};

			AllTagTargetTypes = new[] { TagTargetTypes.Album, TagTargetTypes.Artist, TagTargetTypes.Song, TagTargetTypes.Event }
				.ToDictionary(t => t, GetTagTargetTypeName);
		}

		public TagForEditContract ToContract()
		{
			return new TagForEditContract
			{
				Id = Id,
				Name = Name,
				Names = Names,
				CategoryName = CategoryName ?? string.Empty,
				DefaultNameLanguage = DefaultNameLanguage,
				Description = Description,
				HideFromSuggestions = HideFromSuggestions,
				Parent = Parent,
				RelatedTags = RelatedTags,
				Status = Status,
				Targets = Targets,
				UpdateNotes = UpdateNotes ?? string.Empty,
				WebLinks = WebLinks
			};
		}
	}
}