using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumIdentifierContract
	{

		public AlbumIdentifierContract() { }

		public AlbumIdentifierContract(AlbumIdentifier identifier)
		{
			Value = identifier.Value;
		}

		[DataMember]
		public string Value { get; set; }

	}

}
