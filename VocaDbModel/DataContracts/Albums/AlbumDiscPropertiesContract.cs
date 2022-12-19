#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.Albums
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class AlbumDiscPropertiesContract
	{
		public AlbumDiscPropertiesContract() { }

#nullable enable
		public AlbumDiscPropertiesContract(AlbumDiscProperties discProperties)
		{
			ParamIs.NotNull(() => discProperties);

			DiscNumber = discProperties.DiscNumber;
			Id = discProperties.Id;
			MediaType = discProperties.MediaType;
			Name = discProperties.Name;
		}
#nullable disable

		[DataMember]
		public int DiscNumber { get; set; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public DiscMediaType MediaType { get; init; }

		[DataMember]
		public string Name { get; init; }
	}
}
