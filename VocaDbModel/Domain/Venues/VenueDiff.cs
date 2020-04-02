using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Venues {

	public class VenueDiff : EntryDiff<VenueEditableFields> {

		public VenueDiff() : base(true) { }
		public VenueDiff(bool isSnapshot) : base(isSnapshot) { }
		public VenueDiff(VenueEditableFields changedFields) : base(changedFields) { }

		public EnumFieldAccessor<VenueEditableFields> Address => Field(VenueEditableFields.Address);
		public EnumFieldAccessor<VenueEditableFields> AddressCountryCode => Field(VenueEditableFields.AddressCountryCode);
		public EnumFieldAccessor<VenueEditableFields> Coordinates => Field(VenueEditableFields.Coordinates);
		public EnumFieldAccessor<VenueEditableFields> Description => Field(VenueEditableFields.Description);
		public EnumFieldAccessor<VenueEditableFields> OriginalName => Field(VenueEditableFields.OriginalName);
		public EnumFieldAccessor<VenueEditableFields> Names => Field(VenueEditableFields.Names);
		public EnumFieldAccessor<VenueEditableFields> Status => Field(VenueEditableFields.Status);
		public EnumFieldAccessor<VenueEditableFields> WebLinks => Field(VenueEditableFields.WebLinks);

		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;

	}

}
