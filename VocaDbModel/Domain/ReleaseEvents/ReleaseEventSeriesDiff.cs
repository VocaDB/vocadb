using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents {
	
	public class ReleaseEventSeriesDiff : EntryDiff<ReleaseEventSeriesEditableFields> {

		public ReleaseEventSeriesDiff() : base(true) { }
		public ReleaseEventSeriesDiff(bool isSnapshot) : base(isSnapshot) { }
		public ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields changedFields) : base(changedFields) { }

		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Description => Field(ReleaseEventSeriesEditableFields.Description);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Name => Field(ReleaseEventSeriesEditableFields.Name);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Names => Field(ReleaseEventSeriesEditableFields.Names);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Picture => Field(ReleaseEventSeriesEditableFields.Picture);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> WebLinks => Field(ReleaseEventSeriesEditableFields.WebLinks);

	}

}
