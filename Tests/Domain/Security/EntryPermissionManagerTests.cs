using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Security {

	/// <summary>
	/// Unit tests for <see cref="EntryPermissionManager"/>.
	/// </summary>
	[TestClass]
	public class EntryPermissionManagerTests {

		private Artist artist;
		private User user;

		private void TestCanDelete(bool expected, EntryStatus entryStatus = EntryStatus.Finished, UserGroupId userGroup = UserGroupId.Regular) {

			artist.Status = entryStatus;
			user.GroupId = userGroup;

			var result = EntryPermissionManager.CanDelete(new FakePermissionContext(user), artist);
			Assert.AreEqual(expected, result, "result");

		}

		private void TestCanEdit(bool expected, EntryStatus entryStatus = EntryStatus.Finished, UserGroupId userGroup = UserGroupId.Regular) {

			artist.Status = entryStatus;
			user.GroupId = userGroup;

			var result = EntryPermissionManager.CanEdit(new FakePermissionContext(user), artist);
			Assert.AreEqual(expected, result, "result");

		}

		[TestInitialize]
		public void SetUp() {

			var anotherUser = CreateEntry.User(id: 2);
			artist = CreateEntry.Artist(ArtistType.Producer);
			ArchivedArtistVersion.Create(artist, new ArtistDiff(), new AgentLoginData(anotherUser), ArtistArchiveReason.Created, string.Empty);
			user = CreateEntry.User(id: 1);

		}

		[TestMethod]
		public void CanDelete_NormalUser() {

			TestCanDelete(false, EntryStatus.Finished);

		}

		[TestMethod]
		public void CanDelete_TrustedUser() {

			TestCanDelete(true, EntryStatus.Finished, UserGroupId.Trusted);

		}

		[TestMethod]
		public void CanDelete_TrustedUser_LockedEntry() {

			TestCanDelete(false, EntryStatus.Locked, UserGroupId.Trusted);

		}

		[TestMethod]
		public void CanEdit_NormalUser() {				
			
			TestCanEdit(true, EntryStatus.Finished);

		}

		[TestMethod]
		public void CanEdit_ApprovedEntry() {
			
			TestCanEdit(false, EntryStatus.Approved);

		}

		[TestMethod]
		public void CanEdit_LimitedUser() {
			
			TestCanEdit(false, userGroup: UserGroupId.Limited);

		}

		[TestMethod]
		public void CanEdit_ApprovedEntry_TrustedUser() {
			
			TestCanEdit(true, EntryStatus.Approved, UserGroupId.Trusted);

		}

		[TestMethod]
		public void CanEdit_ApprovedEntry_OwnerUser() {
			
			var artist = CreateEntry.Artist(ArtistType.Producer);			
			var user = CreateEntry.User();

			artist.Status = EntryStatus.Approved;
			user.GroupId = UserGroupId.Regular;

			user.AddOwnedArtist(artist);

			var result = EntryPermissionManager.CanEdit(new FakePermissionContext(user), artist);

			Assert.IsTrue(result, "result");

		}

	}

}
