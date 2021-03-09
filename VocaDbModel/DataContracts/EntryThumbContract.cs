#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryThumbContract : IEntryImageInformation
	{
		private readonly ImagePurpose _purpose;
		ImagePurpose IEntryImageInformation.Purpose => _purpose;

		public EntryThumbContract() { }

		public EntryThumbContract(EntryThumb entryThumb)
		{
			EntryType = entryThumb.EntryType;
			Id = entryThumb.Id;
			Mime = entryThumb.Mime;
			_purpose = entryThumb.Purpose;
			Version = entryThumb.Version;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryType EntryType { get; init; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Mime { get; init; }

		[DataMember]
		public int Version { get; init; }
	}
}
