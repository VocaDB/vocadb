using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.UseCases
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class EntryIdAndVersionContract
	{
		public EntryIdAndVersionContract() { }

		public EntryIdAndVersionContract(int id, int version)
		{
			Id = id;
			Version = version;
		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public int Version { get; set; }
	}
}
