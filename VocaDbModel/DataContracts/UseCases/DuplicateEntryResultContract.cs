using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace VocaDb.Model.DataContracts.UseCases
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class DuplicateEntryResultContract<T> where T : struct
	{

		public DuplicateEntryResultContract(EntryRefWithCommonPropertiesContract entry, T matchProperty)
		{

			Entry = entry;
			MatchProperty = matchProperty;

		}

		[DataMember]
		public EntryRefWithCommonPropertiesContract Entry { get; set; }

		[JsonConverter(typeof(StringEnumConverter))]
		[DataMember]
		public T MatchProperty { get; set; }

	}

}
