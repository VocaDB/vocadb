using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.ReleaseEvents {

	public class ReleaseEventDiff : EntryDiff<ReleaseEventEditableFields> {

		public ReleaseEventDiff() : base(true) {}
		public ReleaseEventDiff(bool isSnapshot) : base(isSnapshot) { }
		public ReleaseEventDiff(ReleaseEventEditableFields changedFields) : base(changedFields) { }

		public EnumFieldAccessor<ReleaseEventEditableFields> Artists => Field(ReleaseEventEditableFields.Artists);
		public EnumFieldAccessor<ReleaseEventEditableFields> Category => Field(ReleaseEventEditableFields.Category);
		public EnumFieldAccessor<ReleaseEventEditableFields> Date => Field(ReleaseEventEditableFields.Date);
		public EnumFieldAccessor<ReleaseEventEditableFields> Description => Field(ReleaseEventEditableFields.Description);
		public EnumFieldAccessor<ReleaseEventEditableFields> MainPicture => Field(ReleaseEventEditableFields.MainPicture);
		public EnumFieldAccessor<ReleaseEventEditableFields> Names => Field(ReleaseEventEditableFields.Names);
		public EnumFieldAccessor<ReleaseEventEditableFields> OriginalName => Field(ReleaseEventEditableFields.OriginalName);
		public EnumFieldAccessor<ReleaseEventEditableFields> PVs => Field(ReleaseEventEditableFields.PVs);
		public EnumFieldAccessor<ReleaseEventEditableFields> Series => Field(ReleaseEventEditableFields.Series);
		public EnumFieldAccessor<ReleaseEventEditableFields> SeriesNumber => Field(ReleaseEventEditableFields.SeriesNumber);
		public EnumFieldAccessor<ReleaseEventEditableFields> SeriesSuffix => Field(ReleaseEventEditableFields.SeriesSuffix);
		public EnumFieldAccessor<ReleaseEventEditableFields> SongList => Field(ReleaseEventEditableFields.SongList);
		public EnumFieldAccessor<ReleaseEventEditableFields> Status => Field(ReleaseEventEditableFields.Status);
		public EnumFieldAccessor<ReleaseEventEditableFields> Venue => Field(ReleaseEventEditableFields.VenueEntry);
		public EnumFieldAccessor<ReleaseEventEditableFields> VenueName => Field(ReleaseEventEditableFields.Venue);
		public EnumFieldAccessor<ReleaseEventEditableFields> WebLinks => Field(ReleaseEventEditableFields.WebLinks);

		public virtual bool IncludeArtists => IsSnapshot || Artists.IsChanged;
		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludePVs => IsSnapshot || PVs.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;

	}

}
