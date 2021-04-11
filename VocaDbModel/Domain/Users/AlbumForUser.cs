#nullable disable

using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Users
{
	public class AlbumForUser : IAlbumLink, IEntryWithIntId
	{
		public const int NotRated = 0;

		private Album _album;
		private User _user;

		public AlbumForUser()
		{
			MediaType = MediaType.PhysicalDisc;
			Rating = NotRated;
			PurchaseStatus = PurchaseStatus.Owned;
		}

		public AlbumForUser(User user, Album album, PurchaseStatus status, MediaType mediaType, int rating)
			: this()
		{
			User = user;
			Album = album;
			PurchaseStatus = status;
			MediaType = mediaType;
			Rating = rating;
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

		public virtual MediaType MediaType { get; set; }

		public virtual PurchaseStatus PurchaseStatus { get; set; }

		/// <summary>
		/// Rating score, 0-5 (0 = no rating).
		/// </summary>
		public virtual int Rating { get; set; }

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
			Album.UserCollections.Remove(this);
			User.AllAlbums.Remove(this);
			Album.UpdateRatingTotals();
		}

#nullable enable
		public virtual bool Equals(AlbumForUser? another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return Id == another.Id;
		}

		public override bool Equals(object? obj) => Equals(obj as AlbumForUser);
#nullable disable

		public override int GetHashCode() => base.GetHashCode();

#nullable enable
		public virtual void Move(Album target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Album))
				return;

			Album.UserCollections.Remove(this);
			_album.UpdateRatingTotals();
			Album = target;
			target.UserCollections.Add(this);
			target.UpdateRatingTotals();
		}

		public override string ToString() => $"{Album} for {User}";
#nullable disable
	}
}
