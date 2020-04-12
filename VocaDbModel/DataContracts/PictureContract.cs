using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	/// <summary>
	/// Data contract for a single picture.
	/// </summary>
	[DataContract(Namespace = Schemas.VocaDb)]
	public class PictureContract {

		public PictureContract(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		public PictureContract(PictureData pictureData, string mime, ImageSize requestedSize) {

			ParamIs.NotNull(() => pictureData);

			Bytes = pictureData.GetBytes(requestedSize);
			Mime = mime;

		}

		[DataMember]
		public Byte[] Bytes { get; set; }

		[DataMember]
		public string Mime { get; set; }

	}

}
