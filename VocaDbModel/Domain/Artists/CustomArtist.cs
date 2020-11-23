namespace VocaDb.Model.Domain.Artists
{
	public class CustomArtist : IArtistLinkWithRoles
	{
		public CustomArtist(string name, ArtistRoles roles)
		{
			Name = name;
			Roles = roles;
		}

		public bool IsSupport
		{
			get { return false; }
		}

		public string Name { get; set; }

		public ArtistRoles Roles { get; set; }

		public Artist Artist
		{
			get
			{
				return null;
			}
		}
	}
}
