using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums {

	public class ReleaseEventDiff : EntryDiff<ReleaseEventEditableFields> {

		public ReleaseEventDiff() : base(true) {}

		public EnumFieldAccessor<ReleaseEventEditableFields> Date => Field(ReleaseEventEditableFields.Date);

		public EnumFieldAccessor<ReleaseEventEditableFields> Description => Field(ReleaseEventEditableFields.Description);

		public EnumFieldAccessor<ReleaseEventEditableFields> Name => Field(ReleaseEventEditableFields.Name);

		public EnumFieldAccessor<ReleaseEventEditableFields> Series => Field(ReleaseEventEditableFields.Series);

		public EnumFieldAccessor<ReleaseEventEditableFields> SeriesNumber => Field(ReleaseEventEditableFields.SeriesNumber);

		public EnumFieldAccessor<ReleaseEventEditableFields> SeriesSuffix => Field(ReleaseEventEditableFields.SeriesSuffix);

	}

}
