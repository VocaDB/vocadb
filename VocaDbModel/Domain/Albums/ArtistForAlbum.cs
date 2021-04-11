#nullable disable

using System;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Albums
{
	public class ArtistForAlbum : IArtistLinkWithRoles, IEquatable<ArtistForAlbum>, IEntryWithIntId
	{
		private Album _album;
		private string _notes;

		public ArtistForAlbum()
		{
			IsSupport = false;
			Notes = string.Empty;
			Roles = ArtistRoles.Default;
		}

		public ArtistForAlbum(Album album, Artist artist, bool support, ArtistRoles roles)
			: this()
		{
			Album = album;
			Artist = artist;
			IsSupport = support;
			Roles = roles;
		}

		public ArtistForAlbum(Album album, string name, bool support, ArtistRoles roles)
			: this()
		{
			Album = album;
			IsSupport = support;
			Name = name;
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

		public virtual Artist Artist { get; set; }

		public virtual ArtistCategories ArtistCategories => ArtistHelper.GetCategories(this);

		public virtual string ArtistToStringOrName => Artist?.ToString() ?? Name;

		public virtual ArtistRoles EffectiveRoles => (Roles != ArtistRoles.Default || Artist == null) ? Roles : ArtistHelper.GetOtherArtistRoles(Artist.ArtistType);

		public virtual int Id { get; set; }

		public virtual bool IsSupport { get; set; }

		public virtual string Name { get; set; }

		public virtual string Notes
		{
			get => _notes;
			set
			{
				ParamIs.NotNull(() => value);
				_notes = value;
			}
		}

		public virtual ArtistRoles Roles { get; set; }

#nullable enable
		public virtual bool ArtistLinkEquals(ArtistForAlbum? another)
		{
			if (another == null)
				return false;

			return ((Artist != null && Artist.Equals(another.Artist)) || (Artist == null && another.Artist == null && Name == another.Name));
		}

		public virtual bool ContentEquals(ArtistForAlbumContract? contract)
		{
			if (contract == null)
				return false;

			var realNewName = contract.IsCustomName ? contract.Name : null;

			return (IsSupport == contract.IsSupport && Roles == contract.Roles && Name == realNewName);
		}
#nullable disable

		public virtual void Delete()
		{
			Album.DeleteArtistForAlbum(this);
		}

#nullable enable
		public virtual bool Equals(ArtistForAlbum? another)
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
			return Equals(obj as ArtistForAlbum);
		}
#nullable disable

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

#nullable enable
		public virtual void Move(Album target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Album))
				return;

			Album.AllArtists.Remove(this);
			Album = target;
			target.AllArtists.Add(this);
		}

		public virtual void Move(Artist target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist.AllAlbums.Remove(this);
			Artist = target;
			target.AllAlbums.Add(this);
		}

		public override string ToString()
		{
			return $"{ArtistToStringOrName} for {Album} [{Id}]";
		}
#nullable disable
	}
}
