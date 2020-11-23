using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Albums
{
	public class AlbumDiff : EntryDiff<AlbumEditableFields>
	{
		public AlbumDiff() : this(true) { }
		public AlbumDiff(bool isSnapshot) : base(isSnapshot) { }

		public EnumFieldAccessor<AlbumEditableFields> Artists => Field(AlbumEditableFields.Artists);
		public EnumFieldAccessor<AlbumEditableFields> Cover => Field(AlbumEditableFields.Cover);
		public EnumFieldAccessor<AlbumEditableFields> Description => Field(AlbumEditableFields.Description);
		public EnumFieldAccessor<AlbumEditableFields> Discs => Field(AlbumEditableFields.Discs);
		public EnumFieldAccessor<AlbumEditableFields> DiscType => Field(AlbumEditableFields.DiscType);
		public EnumFieldAccessor<AlbumEditableFields> Identifiers => Field(AlbumEditableFields.Identifiers);
		public EnumFieldAccessor<AlbumEditableFields> Names => Field(AlbumEditableFields.Names);
		public EnumFieldAccessor<AlbumEditableFields> OriginalName => Field(AlbumEditableFields.OriginalName);
		public EnumFieldAccessor<AlbumEditableFields> OriginalRelease => Field(AlbumEditableFields.OriginalRelease);
		public EnumFieldAccessor<AlbumEditableFields> Pictures => Field(AlbumEditableFields.Pictures);
		public EnumFieldAccessor<AlbumEditableFields> PVs => Field(AlbumEditableFields.PVs);
		public EnumFieldAccessor<AlbumEditableFields> Status => Field(AlbumEditableFields.Status);
		public EnumFieldAccessor<AlbumEditableFields> Tracks => Field(AlbumEditableFields.Tracks);
		public EnumFieldAccessor<AlbumEditableFields> WebLinks => Field(AlbumEditableFields.WebLinks);

		public virtual bool IncludeArtists => IsSnapshot || Artists.IsChanged;
		public virtual bool IncludeCover => Cover.IsChanged;
		public virtual bool IncludeDescription => IsSnapshot || Description.IsChanged;
		public virtual bool IncludeDiscs => IsSnapshot || Discs.IsChanged;
		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludePictures => IsSnapshot || Pictures.IsChanged;
		public virtual bool IncludePVs => IsSnapshot || PVs.IsChanged;
		public virtual bool IncludeTracks => IsSnapshot || Tracks.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;

		public override bool IsIncluded(AlbumEditableFields field)
		{
			return (field != AlbumEditableFields.Cover ? base.IsIncluded(field) : IncludeCover);
		}
	}
}
