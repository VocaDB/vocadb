#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Database.Repositories
{
	/// <summary>
	/// Repository for <see cref="Artist"/>.
	/// </summary>
	public interface IArtistRepository : IRepository<Artist>
	{
	}
}
