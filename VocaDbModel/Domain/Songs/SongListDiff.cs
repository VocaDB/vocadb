using VocaDb.Model.Domain.Versioning;

namespace VocaDb.Model.Domain.Songs {

	public class SongListDiff : EntryDiff<SongListEditableFields> {

		private EnumFieldAccessor<SongListEditableFields> Field(SongListEditableFields field) {
			return new EnumFieldAccessor<SongListEditableFields>(ChangedFields, field);
		}

		public SongListDiff() {
			IsSnapshot = true;
		}

		public EnumFieldAccessor<SongListEditableFields> Description {
			get {
				return Field(SongListEditableFields.Description);
			}
		} 

		public EnumFieldAccessor<SongListEditableFields> FeaturedCategory {
			get {
				return Field(SongListEditableFields.FeaturedCategory);
			}
		} 

		public EnumFieldAccessor<SongListEditableFields> Name {
			get {
				return Field(SongListEditableFields.Name);
			}
		} 

		public EnumFieldAccessor<SongListEditableFields> Songs {
			get {
				return Field(SongListEditableFields.Songs);
			}
		} 

		public EnumFieldAccessor<SongListEditableFields> Status {
			get {
				return Field(SongListEditableFields.Status);
			}
		} 

		public EnumFieldAccessor<SongListEditableFields> Thumbnail {
			get {
				return Field(SongListEditableFields.Thumbnail);
			}
		} 

	}

}
