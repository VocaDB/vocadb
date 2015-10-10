using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums {

	public class AlbumDiscPropertiesContract {

		public AlbumDiscPropertiesContract() { }

		public AlbumDiscPropertiesContract(AlbumDiscProperties discProperties) {
			
			ParamIs.NotNull(() => discProperties);

			DiscNumber = discProperties.DiscNumber;
			Id = discProperties.Id;
			MediaType = discProperties.MediaType;
			Name = discProperties.Name;

		}

		public int DiscNumber { get; set; }

		public int Id { get; set; }

		public DiscMediaType MediaType { get; set; }

		public string Name { get; set; }

	}

}
