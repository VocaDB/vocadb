using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User
{

	/// <summary>
	/// Query parameters for users.
	/// </summary>
	public class UserQueryParams
	{

		public CommonSearchParams Common { get; set; }

		public UserGroupId Group { get; set; }

		public bool IncludeDisabled { get; set; }

		public DateTime? JoinDateAfter { get; set; }

		public DateTime? JoinDateBefore { get; set; }

		public string KnowsLanguage { get; set; }

		public bool OnlyVerifiedArtists { get; set; }

		public PagingProperties Paging { get; set; }

		public UserSortRule Sort { get; set; }

	}

}
