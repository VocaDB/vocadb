using System;
using VocaDb.Model.Domain;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts {

	// TODO: this class isn't really in use anymore and should be replaced
	[DataContract(Namespace = Schemas.VocaDb)]
	public class PictureDataContract {

		public PictureDataContract() { }

		public PictureDataContract(Byte[] bytes, string mime) {
			Bytes = bytes;
			Mime = mime;
		}

		[DataMember]
		public Byte[] Bytes { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public PictureThumbContract Thumb250 { get; set; }

	}

	[DataContract(Namespace = Schemas.VocaDb)]
	public class PictureThumbContract {

		public PictureThumbContract() {}

		public PictureThumbContract(byte[] bytes, int size) {

			Bytes = bytes;
			Size = size;

		}

		public PictureThumbContract(PictureThumb thumb) {
			
			ParamIs.NotNull(() => thumb);

			Bytes = thumb.Bytes;
			Size = thumb.Size;

		}

		[DataMember]
		public virtual Byte[] Bytes { get; set; }

		[DataMember]
		public virtual int Size { get; set; }

	}

}
