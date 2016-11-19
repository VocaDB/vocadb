using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class SongDiff : EntryDiff<SongEditableFields> {

		public SongDiff() : this(true) { }
		public SongDiff(bool isSnapshot) : base(isSnapshot) {}

		public EnumFieldAccessor<SongEditableFields> Artists => Field(SongEditableFields.Artists);
		public EnumFieldAccessor<SongEditableFields> Length => Field(SongEditableFields.Length);
		public EnumFieldAccessor<SongEditableFields> Lyrics => Field(SongEditableFields.Lyrics);
		public EnumFieldAccessor<SongEditableFields> Names => Field(SongEditableFields.Names);
		public EnumFieldAccessor<SongEditableFields> Notes => Field(SongEditableFields.Notes);
		public EnumFieldAccessor<SongEditableFields> OriginalName => Field(SongEditableFields.OriginalName);
		public EnumFieldAccessor<SongEditableFields> OriginalVersion => Field(SongEditableFields.OriginalVersion);
		public EnumFieldAccessor<SongEditableFields> PublishDate => Field(SongEditableFields.PublishDate);
		public EnumFieldAccessor<SongEditableFields> PVs => Field(SongEditableFields.PVs);
		public EnumFieldAccessor<SongEditableFields> ReleaseEvent => Field(SongEditableFields.ReleaseEvent);
		public EnumFieldAccessor<SongEditableFields> SongType => Field(SongEditableFields.SongType);
		public EnumFieldAccessor<SongEditableFields> Status => Field(SongEditableFields.Status);
		public EnumFieldAccessor<SongEditableFields> WebLinks => Field(SongEditableFields.WebLinks);

		public virtual bool IncludeArtists => IsSnapshot || Artists.IsChanged;
		public virtual bool IncludeLyrics => IsSnapshot || Lyrics.IsChanged;
		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludePVs => IsSnapshot || PVs.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;

	}

}
