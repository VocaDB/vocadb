using System.Linq;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Service.QueryableExtenders {

	/// <summary>
	/// Query extensions for <see cref="IArtistLinkWithRoles"/>.
	/// </summary>
	public static class ArtistLinkWithRolesQueryableExtender {

		/// <summary>
		/// Filters query by excluding entries with the specified artist role.
		/// </summary>
		public static IQueryable<T> WhereDoesNotHaveRole<T>(this IQueryable<T> query, ArtistRoles role) where T : IArtistLinkWithRoles {

			return query.Where(q => (q.Roles & role) == ArtistRoles.Default);

		}

	}

}
