namespace VocaDb.Model.Domain.Artists
{
	/// <summary>
	/// Artist link with roles, including the support status.
	/// </summary>
	public interface IArtistLinkWithRoles : IArtistLink
	{
		bool IsSupport { get; }

		string Name { get; }

		ArtistRoles Roles { get; }
	}
}
