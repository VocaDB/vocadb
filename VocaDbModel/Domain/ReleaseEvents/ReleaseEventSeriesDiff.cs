#nullable disable

using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents
{
	public class ReleaseEventSeriesDiff : EntryDiff<ReleaseEventSeriesEditableFields>
	{
		public ReleaseEventSeriesDiff() : base(true) { }
		public ReleaseEventSeriesDiff(bool isSnapshot) : base(isSnapshot) { }
		public ReleaseEventSeriesDiff(ReleaseEventSeriesEditableFields changedFields) : base(changedFields) { }

		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Category => Field(ReleaseEventSeriesEditableFields.Category);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Description => Field(ReleaseEventSeriesEditableFields.Description);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> OriginalName => Field(ReleaseEventSeriesEditableFields.OriginalName);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Names => Field(ReleaseEventSeriesEditableFields.Names);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Picture => Field(ReleaseEventSeriesEditableFields.Picture);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> Status => Field(ReleaseEventSeriesEditableFields.Status);
		public EnumFieldAccessor<ReleaseEventSeriesEditableFields> WebLinks => Field(ReleaseEventSeriesEditableFields.WebLinks);

		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;
	}
}
