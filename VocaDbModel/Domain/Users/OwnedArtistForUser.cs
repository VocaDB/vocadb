using System.Diagnostics.CodeAnalysis;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Users;

/// <summary>
/// User is a verified owner of an artist entry.
/// </summary>
public class OwnedArtistForUser : IEntryWithIntId, IArtistForUser
{
	public static CollectionDiff<OwnedArtistForUser, OwnedArtistForUser> Sync(
		IList<OwnedArtistForUser> oldLinks,
		IEnumerable<ArtistForUserForApiContract> newLinks,
		Func<ArtistForUserForApiContract, OwnedArtistForUser> fac
	)
	{
		return CollectionHelper.Sync(oldLinks, newLinks, (n1, n2) => n1.Id == n2.Id, fac);
	}

	private Artist _artist;
	private User _user;

#nullable disable
	public OwnedArtistForUser() { }
#nullable enable

	public OwnedArtistForUser(User user, Artist artist)
	{
		User = user;
		Artist = artist;
	}

	public virtual int Id { get; set; }

	public virtual Artist Artist
	{
		get => _artist;
		[MemberNotNull(nameof(_artist))]
		set
		{
			ParamIs.NotNull(() => value);
			_artist = value;
		}
	}

	public virtual User User
	{
		get => _user;
		[MemberNotNull(nameof(_user))]
		set
		{
			ParamIs.NotNull(() => value);
			_user = value;
		}
	}

	public virtual void Delete()
	{
		User.AllOwnedArtists.Remove(this);
		Artist.OwnerUsers.Remove(this);
	}

	public virtual bool Equals(OwnedArtistForUser? another)
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
		return Equals(obj as OwnedArtistForUser);
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

		Artist.OwnerUsers.Remove(this);
		Artist = target;
		target.OwnerUsers.Add(this);
	}

	public override string ToString()
	{
		return $"Owned {Artist} for {User}";
	}
}
