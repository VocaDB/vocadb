using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	[DataContract]
	public class EntryThumbContract : IPictureWithThumbs, IEntryImageInformation {

		public EntryThumbContract() {}

		public EntryThumbContract(EntryThumb entryThumb) {
			EntryType = entryThumb.EntryType;
			FileName = entryThumb.FileName;
			FileNameThumb = entryThumb.FileNameThumb;
			FileNameSmallThumb = entryThumb.FileNameSmallThumb;
			FileNameTinyThumb = entryThumb.FileNameTinyThumb;
			Id = entryThumb.Id;
			Mime = entryThumb.Mime;
			Version = entryThumb.Version;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryType EntryType { get; set; }

		[DataMember]
		public string FileName { get; set; }

		[DataMember]
		public string FileNameSmallThumb { get; set; }

		[DataMember]
		public string FileNameThumb { get; set; }

		[DataMember]
		public string FileNameTinyThumb { get; set; }

		[DataMember]
		public int Id { get; set;}

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public int Version { get; set;}

	}
}
