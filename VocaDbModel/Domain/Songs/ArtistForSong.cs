#nullable disable

using System;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Songs
{
	public class ArtistForSong : IEquatable<ArtistForSong>, IArtistLinkWithRoles, ISongLink, IEntryWithIntId
	{
		private string _notes;
		private Song _song;

		public ArtistForSong()
		{
			IsSupport = false;
			Notes = string.Empty;
		}

		public ArtistForSong(Song song, Artist artist, bool support, ArtistRoles roles)
			: this()
		{
			Song = song;
			Artist = artist;
			IsSupport = support;
			Roles = roles;
		}

		public ArtistForSong(Song song, string name, bool support, ArtistRoles roles)
			: this()
		{
			Song = song;
			IsSupport = support;
			Name = name;
			Roles = roles;
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

		public virtual Song Song
		{
			get => _song;
			set
			{
				ParamIs.NotNull(() => value);
				_song = value;
			}
		}

		public virtual bool ArtistLinkEquals(ArtistForSong another)
		{
			if (another == null)
				return false;

			return ((Artist != null && Artist.Equals(another.Artist)) || (Artist == null && another.Artist == null && Name == another.Name));
		}

		public virtual bool ContentEquals(ArtistForSongContract contract)
		{
			if (contract == null)
				return false;

			var realNewName = contract.IsCustomName ? contract.Name : null;

			return (IsSupport == contract.IsSupport && Roles == contract.Roles && Name == realNewName);
		}

		public virtual void Delete()
		{
			if (Artist != null)
				Artist.AllSongs.Remove(this);

			Song.AllArtists.Remove(this);
			Song.UpdateArtistString();
		}

		public virtual bool Equals(ArtistForSong another)
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
			return Equals(obj as ArtistForSong);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public virtual void Move(Artist target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			if (Artist != null)
				Artist.AllSongs.Remove(this);

			Artist = target;
			target.AllSongs.Add(this);
		}

		public virtual void Move(Song target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Song))
				return;

			Song.AllArtists.Remove(this);
			Song = target;
			target.AllArtists.Add(this);
		}

		public override string ToString()
		{
			return $"{ArtistToStringOrName} for {Song}";
		}
	}
}
