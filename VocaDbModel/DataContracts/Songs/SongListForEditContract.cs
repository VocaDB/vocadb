using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.Songs {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListForEditContract : SongListContract {

		public SongListForEditContract() {
			SongLinks = new SongInListEditContract[] {};
		}

		public SongListForEditContract(SongList songList, IUserPermissionContext permissionContext)
			: base(songList, permissionContext) {

			SongLinks = songList.SongLinks
				.OrderBy(s => s.Order)
				.Select(s => new SongInListEditContract(s, permissionContext.LanguagePreference))
				.ToArray();

		}

		[DataMember]
		public SongInListEditContract[] SongLinks { get; set; }

	}

}
