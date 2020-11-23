using System;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts
{
	// TODO: this class isn't really in use anymore and should be replaced
	[DataContract(Namespace = Schemas.VocaDb)]
	public class PictureDataContract
	{
		public PictureDataContract() { }

		public PictureDataContract(Byte[] bytes, string mime)
		{
			Bytes = bytes;
			Mime = mime;
		}

		[DataMember]
		public Byte[] Bytes { get; set; }

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public Byte[] Thumb250 { get; set; }
	}
}
