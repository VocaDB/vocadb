#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Albums
{
	public class OtherArtistForAlbum
	{
		private Album _album;
		private string _name;

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
			get => _album;
			set
			{
				ParamIs.NotNull(() => value);
				_album = value;
			}
		}

		public virtual int Id { get; set; }

		public virtual string Name
		{
			get => _name;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_name = value;
			}
		}

		public virtual bool IsSupport { get; set; }

		public virtual ArtistRoles Roles { get; set; }

#nullable enable
		public virtual bool Equals(OtherArtistForAlbum? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as OtherArtistForAlbum);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
#nullable disable
	}
}
