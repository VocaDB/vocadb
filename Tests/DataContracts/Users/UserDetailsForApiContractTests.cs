using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DataContracts.Users;

[TestClass]
public class UserDetailsForApiContractTests
{
	private static readonly PermissionToken[] s_additionalPermissions = new[]
	{
		PermissionToken.AccessManageMenu,
		PermissionToken.Admin,
		PermissionToken.ViewAuditLog,
	};

	private IUserIconFactory _iconFactory = default!;
	private IAggregatedEntryImageUrlFactory _thumbPersister = default!;
	private User _user = default!;
	private User _viewer = default!;

	[TestInitialize]
	public void SetUp()
	{
		_iconFactory = new FakeUserIconFactory();;
		_thumbPersister = new InMemoryImagePersister();

		_user = CreateEntry.User(id: 1, email: "miku@vocadb.net");
		_user.LastLogin = new DateTime(2022, 8, 31, 3, 9, 39);
		_user.AdditionalPermissions.AddAll(s_additionalPermissions);
		_user.Options.LastLoginAddress = "::1";

		_user.OldUsernames.Add(new OldUsername(_user, "miku"));
		_user.OldUsernames.Add(new OldUsername(_user, "rin"));
		_user.OldUsernames.Add(new OldUsername(_user, "luka"));

		_viewer = CreateEntry.User(id: 2);
	}

	private UserDetailsForApiContract CreateContract(UserGroupId viewerGroup)
	{
		_viewer.GroupId = viewerGroup;
		var permissionContext = new FakePermissionContext(_viewer);

		var contract = new UserDetailsForApiContract(
			_user,
			_iconFactory,
			languagePreference: permissionContext.LanguagePreference,
			_thumbPersister,
			permissionContext
		);

		return contract;
	}

	[TestMethod]
	public void LimitedUserCannotViewSensitiveInformation()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Limited);

		contract.AdditionalPermissions.Should().BeNull();
		contract.EffectivePermissions.Should().BeNull();
		contract.Email.Should().BeNull();
		contract.LastLogin.Should().BeNull();
		contract.LastLoginAddress.Should().BeNull();
	}

	[TestMethod]
	public void RegularUserCannotViewSensitiveInformation()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Regular);

		contract.AdditionalPermissions.Should().BeNull();
		contract.EffectivePermissions.Should().BeNull();
		contract.Email.Should().BeNull();
		contract.LastLogin.Should().BeNull();
		contract.LastLoginAddress.Should().BeNull();
	}

	[TestMethod]
	public void TrustedUserCannotViewSensitiveInformation()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Trusted);

		contract.AdditionalPermissions.Should().BeNull();
		contract.EffectivePermissions.Should().BeNull();
		contract.Email.Should().BeNull();
		contract.LastLogin.Should().BeNull();
		contract.LastLoginAddress.Should().BeNull();
	}

	[TestMethod]
	public void ModeratorCanViewSensitiveInformation()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Moderator);

		contract.AdditionalPermissions.Should().BeEquivalentTo(s_additionalPermissions.Select(p => p.Id));
		contract.EffectivePermissions.Should().BeEquivalentTo(
			UserGroup.s_regular.Permissions
				.Concat(s_additionalPermissions)
				.Select(p => p.Id)
		);
		contract.Email.Should().Be("miku@vocadb.net");
		contract.LastLogin.Should().Be(new DateTime(2022, 8, 31, 3, 9, 39));
		contract.LastLoginAddress.Should().Be("::1");
	}

	[TestMethod]
	public void AdminCanViewSensitiveInformation()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Admin);

		contract.AdditionalPermissions.Should().BeEquivalentTo(s_additionalPermissions.Select(p => p.Id));
		contract.EffectivePermissions.Should().BeEquivalentTo(
			UserGroup.s_regular.Permissions
				.Concat(s_additionalPermissions)
				.Select(p => p.Id)
		);
		contract.Email.Should().Be("miku@vocadb.net");
		contract.LastLogin.Should().Be(new DateTime(2022, 8, 31, 3, 9, 39));
		contract.LastLoginAddress.Should().Be("::1");
	}

	[TestMethod]
	public void LimitedUserCannotViewOldUsernames()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Limited);

		contract.OldUsernames.Should().BeNull();
	}

	[TestMethod]
	public void RegularUserCannotViewOldUsernames()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Regular);

		contract.OldUsernames.Should().BeNull();
	}

	[TestMethod]
	public void TrustedUserCannotViewOldUsernames()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Trusted);

		contract.OldUsernames.Should().BeNull();
	}

	[TestMethod]
	public void ModeratorCanViewOldUsernames()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Moderator);

		contract.OldUsernames.Should().NotBeNull();
		contract.OldUsernames.Length.Should().Be(3);
		contract.OldUsernames[0].OldName.Should().Be("miku");
		contract.OldUsernames[1].OldName.Should().Be("rin");
		contract.OldUsernames[2].OldName.Should().Be("luka");
	}

	[TestMethod]
	public void AdminCanViewOldUsernames()
	{
		var contract = CreateContract(viewerGroup: UserGroupId.Admin);

		contract.OldUsernames.Should().NotBeNull();
		contract.OldUsernames.Length.Should().Be(3);
		contract.OldUsernames[0].OldName.Should().Be("miku");
		contract.OldUsernames[1].OldName.Should().Be("rin");
		contract.OldUsernames[2].OldName.Should().Be("luka");
	}
}
