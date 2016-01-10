using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryThumbContract : IEntryImageInformation {

		public EntryThumbContract() {}

		public EntryThumbContract(EntryThumb entryThumb) {
			EntryType = entryThumb.EntryType;
			Id = entryThumb.Id;
			Mime = entryThumb.Mime;
			Version = entryThumb.Version;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryType EntryType { get; set; }

		[DataMember]
		public int Id { get; set;}

		[DataMember]
		public string Mime { get; set; }

		[DataMember]
		public int Version { get; set;}

	}
}
