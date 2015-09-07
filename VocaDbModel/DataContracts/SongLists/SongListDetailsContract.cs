using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.SongLists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() { }

		public SongListDetailsContract(SongList list, IUserPermissionContext userPermissionContext)
			: base(list, userPermissionContext) {}

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

	}
}
