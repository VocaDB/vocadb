#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryRefContract
	{
		public EntryRefContract() { }

		public EntryRefContract(IEntryBase entryBase)
		{
			ParamIs.NotNull(() => entryBase);

			EntryType = entryBase.EntryType;
			Id = entryBase.Id;
		}

		public EntryRefContract(EntryRef entryRef)
		{
			ParamIs.NotNull(() => entryRef);

			EntryType = entryRef.EntryType;
			Id = entryRef.Id;
		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public EntryType EntryType { get; init; }

		[DataMember]
		public int Id { get; set; }
	}
}
