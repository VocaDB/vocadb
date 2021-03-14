#nullable disable

using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Globalization;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags
{
	[DataContract]
	public class TagForEditContract : TagContract
	{
		public TagForEditContract()
		{
			UpdateNotes = string.Empty;
		}

		public TagForEditContract(Tag tag, bool isEmpty, IUserPermissionContext userContext)
			: base(tag, userContext.LanguagePreference)
		{
			CanDelete = EntryPermissionManager.CanDelete(userContext, tag);
			DefaultNameLanguage = tag.TranslatedName.DefaultLanguage;
			Description = new EnglishTranslatedStringContract(tag.Description);
			IsEmpty = isEmpty;
			Names = tag.Names.Select(n => new LocalizedStringWithIdContract(n)).ToArray();
			RelatedTags = tag.RelatedTags.Select(t => new TagBaseContract(t.LinkedTag, userContext.LanguagePreference, false)).ToArray();
			Thumb = (tag.Thumb != null ? new EntryThumbContract(tag.Thumb) : null);
			UpdateNotes = string.Empty;
			WebLinks = tag.WebLinks.Links.Select(w => new WebLinkContract(w)).OrderBy(w => w.DescriptionOrUrl).ToArray();
		}

		[DataMember]
		public bool CanDelete { get; init; }

		[DataMember]
		public ContentLanguageSelection DefaultNameLanguage { get; init; }

		[DataMember]
		public new EnglishTranslatedStringContract Description { get; set; }

		[DataMember]
		public bool IsEmpty { get; init; }

		[DataMember]
		public LocalizedStringWithIdContract[] Names { get; set; }

		[DataMember]
		public TagBaseContract[] RelatedTags { get; init; }

		[DataMember]
		public EntryThumbContract Thumb { get; init; }

		[DataMember]
		public string UpdateNotes { get; init; }

		[DataMember]
		public WebLinkContract[] WebLinks { get; init; }
	}
}
