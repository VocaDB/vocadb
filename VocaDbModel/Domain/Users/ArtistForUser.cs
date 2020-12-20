#nullable disable

using VocaDb.Model.Domain.Artists;

namespace VocaDb.Model.Domain.Users
{
	/// <summary>
	/// User following an artist.
	/// </summary>
	/// <remarks>For owned artists see <see cref="OwnedArtistForUser"/>.</remarks>
	public class ArtistForUser : IArtistLink, IEntryWithIntId
	{
		private Artist _artist;
		private User _user;

		public ArtistForUser()
		{
			SiteNotifications = true;
		}

		public ArtistForUser(User user, Artist artist)
			: this()
		{
			User = user;
			Artist = artist;
		}

		public virtual int Id { get; set; }

		public virtual Artist Artist
		{
			get => _artist;
			set
			{
				ParamIs.NotNull(() => value);
				_artist = value;
			}
		}

		/// <summary>
		/// Send email notification to user for new songs/albums.
		/// </summary>
		public virtual bool EmailNotifications { get; set; }

		/// <summary>
		/// On-site notifications for new songs/albums.
		/// </summary>
		public virtual bool SiteNotifications { get; set; }

		public virtual User User
		{
			get => _user;
			set
			{
				ParamIs.NotNull(() => value);
				_user = value;
			}
		}

		/// <summary>
		/// Deletes this link and performs any necessary bookkeeping.
		/// Link will be removed from collections on both sides and ratings will be updated.
		/// </summary>
		public virtual void Delete()
		{
			User.AllArtists.Remove(this);
			Artist.Users.Remove(this);
		}

		public virtual bool Equals(ArtistForUser another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return Id == another.Id;
		}

		public override bool Equals(object obj)
		{
			return Equals(obj as ArtistForUser);
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

			Artist.Users.Remove(this);
			Artist = target;
			target.Users.Add(this);
		}

		public override string ToString()
		{
			return $"{User} following {Artist}";
		}
	}
}
