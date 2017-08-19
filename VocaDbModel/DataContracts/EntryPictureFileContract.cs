using System.IO;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryPictureFileContract : IEntryPictureFile {

		int IEntryImageInformation.Version => 0;

		public EntryPictureFileContract() { }

		public EntryPictureFileContract(EntryPictureFile picture, IEntryImagePersister imageStore) {

			ParamIs.NotNull(() => picture);

			EntryType = picture.EntryType;
			Id = picture.Id;
			Mime = picture.Mime;
			Name = picture.Name;
			OwnerEntryId = picture.OwnerEntryId;
			ThumbUrl = imageStore.GetUrlAbsolute(picture, ImageSize.Thumb, true);

		}

		public int ContentLength { get; set;}

		[DataMember]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string OriginalFileName { get; set; }

		[DataMember]
		public int OwnerEntryId { get; set; }

		[DataMember]
		public string ThumbUrl { get; set;}

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
