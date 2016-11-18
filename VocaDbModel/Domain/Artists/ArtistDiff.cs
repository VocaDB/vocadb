using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Artists {

	public class ArtistDiff : EntryDiff<ArtistEditableFields> {

		public ArtistDiff() : this(true) { }

		public ArtistDiff(bool isSnapshot) : base(isSnapshot) {}

		public EnumFieldAccessor<ArtistEditableFields> Albums => Field(ArtistEditableFields.Albums);
		public EnumFieldAccessor<ArtistEditableFields> ArtistType => Field(ArtistEditableFields.ArtistType);
		public EnumFieldAccessor<ArtistEditableFields> BaseVoicebank => Field(ArtistEditableFields.BaseVoicebank);
		public EnumFieldAccessor<ArtistEditableFields> Description => Field(ArtistEditableFields.Description);
		public EnumFieldAccessor<ArtistEditableFields> Groups => Field(ArtistEditableFields.Groups);
		public EnumFieldAccessor<ArtistEditableFields> Names => Field(ArtistEditableFields.Names);
		public EnumFieldAccessor<ArtistEditableFields> OriginalName => Field(ArtistEditableFields.OriginalName);
		public EnumFieldAccessor<ArtistEditableFields> Picture => Field(ArtistEditableFields.Picture);
		public EnumFieldAccessor<ArtistEditableFields> Pictures => Field(ArtistEditableFields.Pictures);
		public EnumFieldAccessor<ArtistEditableFields> ReleaseDate => Field(ArtistEditableFields.ReleaseDate);
		public EnumFieldAccessor<ArtistEditableFields> Status => Field(ArtistEditableFields.Status);
		public EnumFieldAccessor<ArtistEditableFields> WebLinks => Field(ArtistEditableFields.WebLinks);

		public virtual bool IncludeDescription => IsSnapshot || Description.IsChanged;
		public virtual bool IncludeNames => IsSnapshot || Names.IsChanged;
		public virtual bool IncludePictures => IsSnapshot || Pictures.IsChanged;
		public virtual bool IncludeWebLinks => IsSnapshot || WebLinks.IsChanged;
		public bool IncludePicture => Picture.IsChanged;

		public override bool IsIncluded(ArtistEditableFields field) {

			return (field != ArtistEditableFields.Picture ? base.IsIncluded(field) : IncludePicture);

		}

	}

}
