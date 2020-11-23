using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Albums
{
	public class OtherArtistForAlbum
	{
		private Album album;
		private string name;

		public OtherArtistForAlbum() { }

		public OtherArtistForAlbum(Album album, string name, bool isSupport, ArtistRoles roles)
		{
			Album = album;
			Name = name;
			IsSupport = isSupport;
			Roles = roles;
		}

		public virtual Album Album
		{
			get { return album; }
			set
			{
				ParamIs.NotNull(() => value);
				album = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name
		{
			get { return name; }
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				name = value;
			}
		}

		public virtual bool IsSupport { get; set; }

		public virtual ArtistRoles Roles { get; set; }

		public virtual bool Equals(OtherArtistForAlbum another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as OtherArtistForAlbum);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}
}
