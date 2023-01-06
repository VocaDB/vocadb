#nullable disable

using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Security;

/// <summary>
/// Unit tests for <see cref="EntryPermissionManager"/>.
/// </summary>
[TestClass]
public class EntryPermissionManagerTests
{
	private Artist _artist;
	private User _user;
	private Artist _verifiedArtist;
	private User _verifiedUser;
	private User _disabledUser;

	private bool CanDelete<T>(User user, T entry) where T : IEntryWithVersions, IEntryWithStatus
	{
		return EntryPermissionManager.CanDelete(new FakePermissionContext(user), entry);
	}

	private void TestCanDelete(bool expected, EntryStatus entryStatus = EntryStatus.Finished, UserGroupId userGroup = UserGroupId.Regular)
	{
		_artist.Status = entryStatus;
		_user.GroupId = userGroup;

		var result = CanDelete(_user, _artist);
		result.Should().Be(expected, "result");
	}

	private void TestCanEdit(bool expected, EntryStatus entryStatus = EntryStatus.Finished, UserGroupId userGroup = UserGroupId.Regular)
	{
		_artist.Status = entryStatus;
		_user.GroupId = userGroup;

		var result = EntryPermissionManager.CanEdit(new FakePermissionContext(_user), _artist);
		result.Should().Be(expected, "result");
	}

	[TestInitialize]
	public void SetUp()
	{
		var anotherUser = CreateEntry.User(id: 2);
		_artist = CreateEntry.Artist(ArtistType.Producer);
		ArchivedArtistVersion.Create(_artist, new ArtistDiff(), new AgentLoginData(anotherUser), ArtistArchiveReason.Created, string.Empty);
		_user = CreateEntry.User(id: 1);

		_verifiedArtist = CreateEntry.Artist(ArtistType.Producer);
		_verifiedUser = CreateEntry.User(id: 3);

		_verifiedArtist.Status = EntryStatus.Approved;
		_verifiedUser.GroupId = UserGroupId.Regular;
		_verifiedUser.AddOwnedArtist(_verifiedArtist);

		_disabledUser = CreateEntry.User(id: 4);
		_disabledUser.Active = false;
	}

	[TestMethod]
	public void CanDelete_NormalUser()
	{
		TestCanDelete(false, EntryStatus.Finished);
	}

	[TestMethod]
	public void CanDelete_TrustedUser()
	{
		TestCanDelete(true, EntryStatus.Finished, UserGroupId.Trusted);
	}

	[TestMethod]
	public void CanDelete_TrustedUser_LockedEntry()
	{
		TestCanDelete(false, EntryStatus.Locked, UserGroupId.Trusted);
	}

	[TestMethod]
	public void CanDelete_Owner_Direct()
	{
		CanDelete(_verifiedUser, _verifiedArtist).Should().BeTrue();
	}

	[TestMethod]
	public void CanDelete_Owner_Transitive()
	{
		var song = CreateEntry.Song();
		song.AddArtist(_verifiedArtist);

		CanDelete(_verifiedUser, song).Should().BeTrue();
	}

	[TestMethod]
	public void CanDelete_Owner_Transitive_Voicebank()
	{
		_verifiedArtist.ArtistType = ArtistType.UTAU;
		var song = CreateEntry.Song();
		song.AddArtist(_verifiedArtist);

		CanDelete(_verifiedUser, song).Should().BeFalse();
	}

	[TestMethod]
	public void CanEdit_NormalUser()
	{
		TestCanEdit(true, EntryStatus.Finished);
	}

	[TestMethod]
	public void CanEdit_ApprovedEntry()
	{
		TestCanEdit(false, EntryStatus.Approved);
	}

	[TestMethod]
	public void CanEdit_LimitedUser()
	{
		TestCanEdit(false, userGroup: UserGroupId.Limited);
	}

	[TestMethod]
	public void CanEdit_ApprovedEntry_TrustedUser()
	{
		TestCanEdit(true, EntryStatus.Approved, UserGroupId.Trusted);
	}

	[TestMethod]
	public void CanEdit_ApprovedEntry_OwnerUser()
	{
		var result = EntryPermissionManager.CanEdit(new FakePermissionContext(_verifiedUser), _verifiedArtist);

		result.Should().BeTrue("result");
	}

	[TestMethod]
	public void LimitedUserCannotViewDisabledUser()
	{
		var user = CreateEntry.User(group: UserGroupId.Limited);
		var actual = EntryPermissionManager.CanViewUser(new FakePermissionContext(user), _disabledUser);
		actual.Should().BeFalse();
	}

	[TestMethod]
	public void RegularUserCannotViewDisabledUser()
	{
		var user = CreateEntry.User(group: UserGroupId.Regular);
		var actual = EntryPermissionManager.CanViewUser(new FakePermissionContext(user), _disabledUser);
		actual.Should().BeFalse();
	}

	[TestMethod]
	public void TrustedUserCannotViewDisabledUser()
	{
		var user = CreateEntry.User(group: UserGroupId.Trusted);
		var actual = EntryPermissionManager.CanViewUser(new FakePermissionContext(user), _disabledUser);
		actual.Should().BeFalse();
	}

	[TestMethod]
	public void ModeratorCanViewDisabledUser()
	{
		var user = CreateEntry.User(group: UserGroupId.Moderator);
		var actual = EntryPermissionManager.CanViewUser(new FakePermissionContext(user), _disabledUser);
		actual.Should().BeTrue();
	}

	[TestMethod]
	public void AdminCanViewViewDisabledUser()
	{
		var user = CreateEntry.User(group: UserGroupId.Admin);
		var actual = EntryPermissionManager.CanViewUser(new FakePermissionContext(user), _disabledUser);
		actual.Should().BeTrue();
	}
}
