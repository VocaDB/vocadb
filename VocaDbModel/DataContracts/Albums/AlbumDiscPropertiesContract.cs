using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumDiscPropertiesContract {

		public AlbumDiscPropertiesContract() { }

		public AlbumDiscPropertiesContract(AlbumDiscProperties discProperties) {
			
			ParamIs.NotNull(() => discProperties);

			DiscNumber = discProperties.DiscNumber;
			Id = discProperties.Id;
			MediaType = discProperties.MediaType;
			Name = discProperties.Name;

		}

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public DiscMediaType MediaType { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
