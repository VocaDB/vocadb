using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.ReleaseEvents;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Model.DataContracts.SongLists {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class SongListDetailsContract : SongListContract {

		public SongListDetailsContract() { }

		public SongListDetailsContract(SongList list, IUserPermissionContext userPermissionContext)
			: base(list, userPermissionContext) {

			Events = list.Events.Select(e => new ReleaseEventContract(e, userPermissionContext.LanguagePreference)).OrderBy(e => e.Date).ThenBy(e => e.Name).ToArray();
			Tags = list.Tags.ActiveUsages.Select(u => new TagUsageForApiContract(u, userPermissionContext.LanguagePreference)).OrderByDescending(u => u.Count).ToArray();

		}

		[DataMember]
		public ReleaseEventContract[] Events { get; set; }

		[DataMember]
		public CommentForApiContract[] LatestComments { get; set; }

		[DataMember]
		public TagUsageForApiContract[] Tags { get; set; }

	}
}
