using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryBaseContract : EntryRefContract
	{
		public EntryBaseContract() { }

		public EntryBaseContract(IEntryBase entry)
			: base(entry)
		{
			DefaultName = entry.DefaultName;
		}

		[DataMember]
		public string DefaultName { get; set; }
	}
}
