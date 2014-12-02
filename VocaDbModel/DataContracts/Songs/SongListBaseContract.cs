using System.Runtime.Serialization;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListBaseContract {

		public SongListBaseContract() {

			Name = string.Empty;

		}

		public SongListBaseContract(SongList songList)
			: this() {

			ParamIs.NotNull(() => songList);

			Id = songList.Id;
			Name = songList.Name;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
