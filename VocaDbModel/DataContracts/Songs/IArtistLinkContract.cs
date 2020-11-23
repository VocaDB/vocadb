using VocaDb.Model.DataContracts.Artists;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.DataContracts.Songs
{
	public interface IArtistLinkContract
	{
		ArtistContract Artist { get; }

		ArtistCategories Categories { get; }

		ArtistRoles EffectiveRoles { get; }

		bool IsSupport { get; }

		string Name { get; }
	}
}
