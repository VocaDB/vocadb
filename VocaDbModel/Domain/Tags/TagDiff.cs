using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Tags {

	public class TagDiff : EntryDiff<TagEditableFields> {

		public TagDiff() : this(true) {}

		public TagDiff(bool isSnapShot) 
			: base(isSnapShot) {}

		public EnumFieldAccessor<TagEditableFields> AliasedTo => Field(TagEditableFields.AliasedTo);

		public EnumFieldAccessor<TagEditableFields> CategoryName => Field(TagEditableFields.CategoryName);

		public EnumFieldAccessor<TagEditableFields> Description => Field(TagEditableFields.Description);

		public EnumFieldAccessor<TagEditableFields> HideFromSuggestions => Field(TagEditableFields.HideFromSuggestions);

		public virtual bool IncludeDescription => IsSnapshot || Description.IsChanged;

		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;

		public virtual bool IncludeRelatedTags => IsSnapshot || RelatedTags.IsChanged;

		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;

		public EnumFieldAccessor<TagEditableFields> Names => Field(TagEditableFields.Names);

		public EnumFieldAccessor<TagEditableFields> OriginalName => Field(TagEditableFields.OriginalName);

		public EnumFieldAccessor<TagEditableFields> Parent => Field(TagEditableFields.Parent);

		public EnumFieldAccessor<TagEditableFields> Picture => Field(TagEditableFields.Picture);

		public EnumFieldAccessor<TagEditableFields> RelatedTags => Field(TagEditableFields.RelatedTags);

		public EnumFieldAccessor<TagEditableFields> Status => Field(TagEditableFields.Status);

		public EnumFieldAccessor<TagEditableFields> WebLinks => Field(TagEditableFields.WebLinks);


	}
}
