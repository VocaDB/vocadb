using System.Runtime.Serialization;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Entry thumbnail for API.
	/// Contains URLs to thumbnails of different sizes.
	/// Does not include URL to original picture at the moment because that is loaded differently.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryThumbForApiContract {

		public EntryThumbForApiContract() { }

		public EntryThumbForApiContract(IEntryImageInformation image, IEntryImagePersister thumbPersister, bool ssl) {

			if (!string.IsNullOrEmpty(image.Mime)) {
				UrlSmallThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.SmallThumb, ssl);
				UrlThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.Thumb, ssl);
				UrlTinyThumb = thumbPersister.GetUrlAbsolute(image, ImageSize.TinyThumb, ssl);				
			}

		}

		[DataMember]
		public string UrlSmallThumb { get; set; }

		[DataMember]
		public string UrlThumb { get; set; }

		[DataMember]
		public string UrlTinyThumb { get; set; }

	}

}
