using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Songs {

	public class SongListsByCategoryContract {

		public SongListsByCategoryContract() { }

		public SongListsByCategoryContract(SongListFeaturedCategory category, IEnumerable<SongList> lists, IUserPermissionContext permissionContext) {

			Category = category;
			Lists = lists.Select(l => new SongListContract(l, permissionContext)).ToArray();

		}

		public SongListFeaturedCategory Category { get; set; }

		public SongListContract[] Lists { get; set; }

	}
}
