using System.IO;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryPictureFileContract : IEntryImageInformation {

		int IEntryImageInformation.Version {
			get { return 0; }
		}

		public EntryPictureFileContract() { }

		public EntryPictureFileContract(EntryPictureFile picture) {

			ParamIs.NotNull(() => picture);

			EntryType = picture.EntryType;
			FileName = picture.FileName;
			Id = picture.Id;
			Mime = picture.Mime;
			Name = picture.Name;
			ThumbUrl = ImageHelper.GetImageUrlThumb(this);

		}

		public int ContentLength { get; set;}

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string ThumbUrl { get; set; }

		/// <summary>
		/// File data stream. Only used for uploads.
		/// </summary>
		public Stream UploadedFile { get; set; }

		public EntryPictureFileContract NullToEmpty() {
			Name = Name ?? string.Empty;
			return this;
		}

	}
}
