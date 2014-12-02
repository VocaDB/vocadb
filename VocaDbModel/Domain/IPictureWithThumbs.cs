namespace VocaDb.Model.Domain {

	public interface IPictureWithThumbs {

		EntryType EntryType { get; }

		string FileName { get; }

		string FileNameThumb { get; }

		string FileNameSmallThumb { get; }

		string FileNameTinyThumb { get; }

	}
}
