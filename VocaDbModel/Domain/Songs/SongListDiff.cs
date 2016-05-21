using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class SongListDiff : EntryDiff<SongListEditableFields> {

		public SongListDiff() : base(true) {}

		public EnumFieldAccessor<SongListEditableFields> Description => Field(SongListEditableFields.Description);

		public EnumFieldAccessor<SongListEditableFields> FeaturedCategory => Field(SongListEditableFields.FeaturedCategory);

		public EnumFieldAccessor<SongListEditableFields> Name => Field(SongListEditableFields.Name);

		public EnumFieldAccessor<SongListEditableFields> Songs => Field(SongListEditableFields.Songs);

		public EnumFieldAccessor<SongListEditableFields> Status => Field(SongListEditableFields.Status);

		public EnumFieldAccessor<SongListEditableFields> Thumbnail => Field(SongListEditableFields.Thumbnail);
	}

}
