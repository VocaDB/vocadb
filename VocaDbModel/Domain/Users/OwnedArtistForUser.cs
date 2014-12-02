using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Users {

	/// <summary>
	/// User is a verified owner of an artist entry.
	/// </summary>
	public class OwnedArtistForUser {

		public static CollectionDiff<OwnedArtistForUser, OwnedArtistForUser> Sync(
			IList<OwnedArtistForUser> oldLinks, IEnumerable<ArtistForUserContract> newLinks, Func<ArtistForUserContract, OwnedArtistForUser> fac) {

			return CollectionHelper.Sync(oldLinks, newLinks, (n1, n2) => n1.Id == n2.Id, fac);

		}

		private Artist artist;
		private User user;

		public OwnedArtistForUser() { }

		public OwnedArtistForUser(User user, Artist artist) {
			User = user;
			Artist = artist;
		}

		public virtual int Id { get; set; }

		public virtual Artist Artist {
			get { return artist; }
			set {
				ParamIs.NotNull(() => value);
				artist = value;
			}
		}

		public virtual User User {
			get { return user; }
			set {
				ParamIs.NotNull(() => value);
				user = value;
			}
		}

		public virtual void Delete() {

			User.AllOwnedArtists.Remove(this);
			Artist.OwnerUsers.Remove(this);

		}

		public virtual bool Equals(OwnedArtistForUser another) {

			if (another == null)
				return false;

			if (ReferenceEquals(this, another))
				return true;

			if (Id == 0)
				return false;

			return this.Id == another.Id;

		}

		public override bool Equals(object obj) {
			return Equals(obj as OwnedArtistForUser);
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public virtual void Move(Artist target) {

			ParamIs.NotNull(() => target);

			if (target.Equals(Artist))
				return;

			Artist.OwnerUsers.Remove(this);
			Artist = target;
			target.OwnerUsers.Add(this);

		}

		public override string ToString() {
			return string.Format("Owned {0} for {1}", Artist, User);
		}

	}
}
