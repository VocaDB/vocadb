namespace VocaDb.Model.Domain.Artists {

	public interface IArtistWithSupport : IArtistLink {

		bool IsSupport { get; }

		string Name { get; }

		ArtistRoles Roles { get; }

	}
}
