#nullable disable

using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListBaseContract : IEntryWithIntId
	{
		public SongListBaseContract()
		{
			Name = string.Empty;
		}

#nullable enable
		public SongListBaseContract(SongList songList)
			: this()
		{
			ParamIs.NotNull(() => songList);

			FeaturedCategory = songList.FeaturedCategory;
			Id = songList.Id;
			Name = songList.Name;
		}
#nullable disable

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public SongListFeaturedCategory FeaturedCategory { get; init; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; init; }
	}
}
