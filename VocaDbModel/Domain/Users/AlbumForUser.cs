using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.Domain.Users
{
	public class AlbumForUser : IAlbumLink, IEntryWithIntId
	{
		public const int NotRated = 0;

		private Album album;
		private User user;

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
			get => album;
			set
			{
				ParamIs.NotNull(() => value);
				album = value;
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
			get => user;
			set
			{
				ParamIs.NotNull(() => value);
				user = value;
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

		public virtual bool Equals(AlbumForUser another)
		{
			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			return this.Id == another.Id;
		}

		public override bool Equals(object obj) => Equals(obj as AlbumForUser);

		public override int GetHashCode() => base.GetHashCode();

		public virtual void Move(Album target)
		{
			ParamIs.NotNull(() => target);

			if (target.Equals(Album))
				return;

			Album.UserCollections.Remove(this);
			album.UpdateRatingTotals();
			Album = target;
			target.UserCollections.Add(this);
			target.UpdateRatingTotals();
		}

		public override string ToString() => string.Format("{0} for {1}", Album, User);
	}
}
