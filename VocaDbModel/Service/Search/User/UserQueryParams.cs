using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Paging;

namespace VocaDb.Model.Service.Search.User
{
	/// <summary>
	/// Query parameters for users.
	/// </summary>
	public sealed record UserQueryParams
	{
		public CommonSearchParams Common { get; init; } = CommonSearchParams.Default;

		public UserGroupId Group { get; init; }

		public bool IncludeDisabled { get; init; }

		public DateTime? JoinDateAfter { get; init; }

		public DateTime? JoinDateBefore { get; init; }

		public string? KnowsLanguage { get; init; }

		public bool OnlyVerifiedArtists { get; init; }

		public PagingProperties Paging { get; init; } = PagingProperties.Default;

		public UserSortRule Sort { get; init; }
	}
}
