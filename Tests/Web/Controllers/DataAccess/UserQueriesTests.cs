using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Comments;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Exceptions;
using VocaDb.Model.Service.Paging;
using VocaDb.Model.Service.Search;
using VocaDb.Model.Service.Search.User;
using VocaDb.Model.Service.Security;
using VocaDb.Model.Service.Security.StopForumSpam;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess {

	/// <summary>
	/// Tests for <see cref="UserQueries"/>.
	/// </summary>
	[TestClass]
	public class UserQueriesTests {

		private const string defaultCulture = "ja-JP";
		private const string defaultHostname = "crypton.jp";
		private UserQueries data;
		private FakeUserMessageMailer mailer;
		private PasswordResetRequest request;
		private FakePermissionContext permissionContext;
		private FakeUserRepository repository;
		private HostCollection softBannedIPs;
		private FakeStopForumSpamClient stopForumSpamClient;
		private User userWithEmail;
		private User userWithoutEmail;

		private User LoggedUser => userWithEmail;

		private void AssertEqual(User expected, UserContract actual) {
			
			Assert.IsNotNull(actual, "Cannot be null");
			Assert.AreEqual(expected.Name, actual.Name, "Name");
			Assert.AreEqual(expected.Id, actual.Id, "Id");

		}

		private void AssertHasAlbum(User user, Album album) {
			Assert.IsTrue(userWithEmail.Albums.Any(a => a.Album == album), "User has album");
		}

		private UserContract CallCreate(string name = "hatsune_miku", string pass = "3939", string email = "", string hostname = defaultHostname, 
			string culture = defaultCulture, TimeSpan? timeSpan = null) {

			return data.Create(name, pass, email, hostname, null, 
				culture,
				timeSpan ?? TimeSpan.FromMinutes(39), softBannedIPs, string.Empty);

		}

		private PartialFindResult<UserContract> CallGetUsers(UserGroupId groupId = UserGroupId.Nothing, string name = null, bool disabled = false, bool verifiedArtists = false, UserSortRule sortRule = UserSortRule.Name, PagingProperties paging = null) {
			var queryParams = new UserQueryParams {
				Common = new CommonSearchParams(SearchTextQuery.Create(name), false, false),
				Group = groupId,
				IncludeDisabled = disabled,
				OnlyVerifiedArtists = verifiedArtists,
				Sort = sortRule,
				Paging = paging ?? new PagingProperties(0, 10, true)
			};
			return data.GetUsers(queryParams, u => new UserContract(u));
		}

		private User GetUserFromRepo(string username) {
			return repository.List<User>().FirstOrDefault(u => u.Name == username);
		}

		private void RefreshLoggedUser() {
			permissionContext.RefreshLoggedUser(repository);
		}

		[TestInitialize]
		public void SetUp() {

			userWithEmail = new User("already_exists", "123", "already_in_use@vocadb.net", PasswordHashAlgorithms.Default) { Id = 123 };
			userWithoutEmail = new User("no_email", "222", string.Empty, PasswordHashAlgorithms.Default) { Id = 321 };
			repository = new FakeUserRepository(userWithEmail, userWithoutEmail);
			repository.Add(userWithEmail.Options);
			permissionContext = new FakePermissionContext(new UserWithPermissionsContract(userWithEmail, ContentLanguagePreference.Default));
			stopForumSpamClient = new FakeStopForumSpamClient();
			mailer = new FakeUserMessageMailer();
			data = new UserQueries(repository, permissionContext, new FakeEntryLinkFactory(), stopForumSpamClient, mailer, 
				new FakeUserIconFactory(), null, new InMemoryImagePersister(), new FakeObjectCache(), new Model.Service.BrandableStrings.BrandableStringsManager(), new EnumTranslations());
			softBannedIPs = new HostCollection();

			request = new PasswordResetRequest(userWithEmail) { Id = Guid.NewGuid() };
			repository.Add(request);

		}

		[TestMethod]
		public void CheckAuthentication() {

			var result = data.CheckAuthentication("already_exists", "123", "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(userWithEmail, result.User);

		}

		[TestMethod]
		public void CheckAuthentication_DifferentCase() {

			userWithEmail.Name = "Already_Exists";
			var result = data.CheckAuthentication("already_exists", "123", "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(userWithEmail, result.User);

		}

		[TestMethod]
		public void CheckAuthentication_WrongPassword() {

			var result = data.CheckAuthentication("already_exists", "3939", "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.InvalidPassword, result.Error, "Error");

		}

		[TestMethod]
		public void CheckAuthentication_NotFound() {

			var result = data.CheckAuthentication("does_not_exist", "3939", "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.NotFound, result.Error, "Error");

		}

		[TestMethod]
		public void CheckAuthentication_Poisoned() {

			userWithEmail.Options.Poisoned = true;
			var result = data.CheckAuthentication(userWithEmail.Name, userWithEmail.Password, "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.AccountPoisoned, result.Error, "Error");

		}

		[TestMethod]
		public void CheckAuthentication_LoginWithEmail() {

			userWithEmail.Options.EmailVerified = true; // For now, logging in with email is allowed only if the email is verified
			var result = data.CheckAuthentication(userWithEmail.Email, "123", "miku@crypton.jp", defaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(userWithEmail, result.User);

		}

		[TestMethod]
		public void ClearRatings() {
		
			userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();
			var album = CreateEntry.Album();
			var song = CreateEntry.Song();
			repository.Save(album);
			repository.Save(song);
			repository.Save(userWithoutEmail.AddAlbum(album, PurchaseStatus.Nothing, MediaType.DigitalDownload, 5));
			repository.Save(userWithoutEmail.AddSongToFavorites(song, SongVoteRating.Favorite));

			data.ClearRatings(userWithoutEmail.Id);

			Assert.AreEqual(0, userWithoutEmail.AllAlbums.Count, "No albums for user");
			Assert.AreEqual(0, userWithoutEmail.FavoriteSongs.Count, "No songs for user");
			Assert.AreEqual(0, album.UserCollections.Count, "Number of users for the album");
			Assert.AreEqual(0, song.UserFavorites.Count, "Number of users for the song");
			Assert.AreEqual(0, album.RatingTotal, "Album RatingTotal");
			Assert.AreEqual(0, song.RatingScore, "Song RatingScore");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void ClearRatings_NoPermission() {
			
			data.ClearRatings(userWithoutEmail.Id);

		}

		[TestMethod]
		public void Create() {

			var name = "hatsune_miku";
			var result = CallCreate(name: name, email: "mikumiku@crypton.jp");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = GetUserFromRepo(name);
			Assert.IsNotNull(user, "User found in repository");
			Assert.AreEqual(name, user.Name, "Name");
			Assert.AreEqual("mikumiku@crypton.jp", user.Email, "Email");
			Assert.AreEqual(UserGroupId.Regular, user.GroupId, "GroupId");
			Assert.IsFalse(repository.List<UserReport>().Any(), "No reports");

			var verificationRequest = repository.List<PasswordResetRequest>().FirstOrDefault(r => r.User.Equals(user));
			Assert.IsNotNull(verificationRequest, "Verification request was created");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void Create_NameAlreadyExists() {

			CallCreate(name: "already_exists");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void Create_NameAlreadyExistsDifferentCase() {

			CallCreate(name: "Already_Exists");

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void Create_EmailAlreadyExists() {

			CallCreate(email: "already_in_use@vocadb.net");

		}

		[TestMethod]
		public void Create_EmailAlreadyExistsButDisabled() {

			userWithEmail.Active = false;
			var result = CallCreate(email: "already_in_use@vocadb.net");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual("hatsune_miku", result.Name, "Name");

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public void Create_InvalidEmailFormat() {

			CallCreate(email: "mikumiku");

		}

		[TestMethod]
		public void Create_MalicousIP() {

			stopForumSpamClient.Response = new SFSResponseContract { Appears = true, Confidence = 99d, Frequency = 100 };
			var result = CallCreate();

			Assert.IsNotNull(result, "result");
			var report = repository.List<UserReport>().FirstOrDefault();
			Assert.IsNotNull(report, "User was reported");
			Assert.AreEqual(UserReportType.MaliciousIP, report.ReportType, "Report type");
			Assert.AreEqual(defaultHostname, report.Hostname, "Hostname");

			var user = GetUserFromRepo(result.Name);
			Assert.AreEqual(UserGroupId.Limited, user.GroupId, "GroupId");
		
		}

		[TestMethod]
		[ExpectedException(typeof(TooFastRegistrationException))]
		public void Create_RegistrationTimeTrigger() {

			CallCreate(timeSpan: TimeSpan.FromSeconds(4));
			Assert.IsFalse(softBannedIPs.Contains(defaultHostname), "Was not banned");

		}

		[TestMethod]
		[ExpectedException(typeof(TooFastRegistrationException))]
		public void Create_RegistrationTimeAndBanTrigger() {

			CallCreate(timeSpan: TimeSpan.FromSeconds(1));
			Assert.IsTrue(softBannedIPs.Contains(defaultHostname), "Was banned");

		}

		[TestMethod]
		public void CreateComment() {

			var sender = userWithEmail;
			var receiver = userWithoutEmail;
			var result = data.CreateComment(receiver.Id, "Hello world");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Hello world", result.Message, "Message");

			var comment = repository.List<Comment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual("Hello world", comment.Message, "Message");
			Assert.AreEqual(sender.Id, comment.Author.Id, "Sender Id");
			Assert.AreEqual(receiver.Id, comment.Entry.Id, "Receiver Id");

			var notificationMsg = string.Format("{0} posted a comment on your profile.\n\n{1}", sender.Name, comment.Message);
			var notification = repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull(notification, "Notification was saved");
			Assert.AreEqual(notificationMsg, notification.Message, "Notification message");
			Assert.AreEqual(receiver.Id, notification.Receiver.Id, "Receiver Id");

		}

		[TestMethod]
		public void CreateTwitter() {

			var name = "hatsune_miku";
			var result = data.CreateTwitter("auth_token", name, "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = GetUserFromRepo(name);
			Assert.IsNotNull(user, "User found in repository");
			Assert.AreEqual(name, user.Name, "Name");
			Assert.AreEqual("mikumiku@crypton.jp", user.Email, "Email");
			Assert.AreEqual(UserGroupId.Regular, user.GroupId, "GroupId");
			Assert.AreEqual("ja-JP", user.Options.LastLoginCulture.CultureCode, "LastLoginCulture");

			Assert.AreEqual("auth_token", user.Options.TwitterOAuthToken, "TwitterOAuthToken");
			Assert.AreEqual(39, user.Options.TwitterId, "TwitterId");
			Assert.AreEqual("Miku_Crypton", user.Options.TwitterName, "TwitterName");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void CreateTwitter_NameAlreadyExists() {

			data.CreateTwitter("auth_token", "already_exists", "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void CreateTwitter_EmailAlreadyExists() {

			data.CreateTwitter("auth_token", "hatsune_miku", "already_in_use@vocadb.net", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public void CreateTwitter_InvalidEmailFormat() {

			data.CreateTwitter("auth_token", "hatsune_miku", "mikumiku", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

		}

		[TestMethod]
		public void DisableUser() {
			
			userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();

			data.DisableUser(userWithoutEmail.Id);

			Assert.AreEqual(false, userWithoutEmail.Active, "User was disabled");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void DisableUser_NoPermission() {

			data.DisableUser(userWithoutEmail.Id);

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void DisableUser_CannotBeDisabled() {

			userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			userWithoutEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();

			data.DisableUser(userWithoutEmail.Id);

		}

		[TestMethod]
		public void GetRatingsByGenre() {

			var fakeTagMock = new Mock<Tag>();
			var fakeTag = fakeTagMock.Object; 
			var vocarock = new Tag("Vocarock", TagCommonCategoryNames.Genres) { Parent = fakeTag };
			var electronic = new Tag("Electronic", TagCommonCategoryNames.Genres) { Parent = fakeTag };
			var trance = new Tag("Trance", TagCommonCategoryNames.Genres) { Parent = electronic };
			repository.Save(vocarock, electronic, trance);
			repository.SaveNames(vocarock, electronic, trance);

			var song1 = CreateEntry.Song(name: "Nebula");
			var song2 = CreateEntry.Song(name: "Anger");
			var song3 = CreateEntry.Song(name: "DYE");
			repository.Add(song1, song2, song3);

			userWithEmail.AddSongToFavorites(song1, SongVoteRating.Favorite);
			userWithEmail.AddSongToFavorites(song2, SongVoteRating.Favorite);
			userWithEmail.AddSongToFavorites(song3, SongVoteRating.Favorite);

			var usage1 = CreateEntry.SongTagUsage(song1, vocarock, userWithEmail);
			var usage2 = CreateEntry.SongTagUsage(song1, trance, userWithEmail);
			var usage3 = CreateEntry.SongTagUsage(song2, vocarock, userWithEmail);
			var usage4 = CreateEntry.SongTagUsage(song2, trance, userWithEmail);
			var usage5 = CreateEntry.SongTagUsage(song3, trance, userWithEmail);
			repository.Add(usage1, usage2, usage3, usage4, usage5);

			var result = data.GetRatingsByGenre(userWithEmail.Id);

			Assert.AreEqual(2, result.Length, "Number of results");
			var first = result[0];
			Assert.AreEqual(electronic.DefaultName, first.Item1, "First result is Electronic");
			Assert.AreEqual(3, first.Item2, "Votes for Electronic");

			var second = result[1];
			Assert.AreEqual(vocarock.DefaultName, second.Item1, "First result is Vocarock");
			Assert.AreEqual(2, second.Item2, "Votes for Vocarock");

		}

		[TestMethod]
		public void GetUsers_NoFilters() {

			var result = CallGetUsers();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(2, result.Items.Length, "Result items");
			Assert.AreEqual(2, result.TotalCount, "Total count");

		}

		[TestMethod]
		public void GetUsers_FilterByName() {

			var result = CallGetUsers(name: "already");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Items.Length, "Result items");
			Assert.AreEqual(1, result.TotalCount, "Total count");
			AssertEqual(userWithEmail, result.Items.First());

		}

		[TestMethod]
		public void GetUsers_Paging() {

			var result = CallGetUsers(paging: new PagingProperties(1, 10, true));
			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Items.Length, "Result items");
			Assert.AreEqual(2, result.TotalCount, "Total count");
			AssertEqual(userWithoutEmail, result.Items.First());

		}

		[TestMethod]
		public void RequestEmailVerification() {
			
			var num = repository.List<PasswordResetRequest>().Count;

			data.RequestEmailVerification(userWithEmail.Id, string.Empty);

			Assert.AreEqual("Verify your email at VocaDB.", mailer.Subject, "Subject");
			Assert.AreEqual(userWithEmail.Email, mailer.ToEmail, "ToEmail");
			Assert.AreEqual(num + 1, repository.List<PasswordResetRequest>().Count, "Number of password reset requests");

		}

		[TestMethod]
		public void RequestPasswordReset() {
			
			var num = repository.List<PasswordResetRequest>().Count;

			data.RequestPasswordReset(userWithEmail.Name, userWithEmail.Email, string.Empty);

			Assert.AreEqual("Password reset requested.", mailer.Subject, "Subject");
			Assert.AreEqual(userWithEmail.Email, mailer.ToEmail, "ToEmail");
			Assert.AreEqual(num + 1, repository.List<PasswordResetRequest>().Count, "Number of password reset requests");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNotFoundException))]
		public void RequestPasswordReset_NotFound() {

			data.RequestPasswordReset(userWithEmail.Name, "notfound@vocadb.net", string.Empty);

		}

		[TestMethod]
		[ExpectedException(typeof(UserNotFoundException))]
		public void RequestPasswordReset_Disabled() {

			userWithEmail.Active = false;
			data.RequestPasswordReset(userWithEmail.Name, userWithEmail.Email, string.Empty);

		}

		[TestMethod]
		public void ResetPassword() {
			
			data.ResetPassword(request.Id, "123");

			var hashed = PasswordHashAlgorithms.Default.HashPassword("123", request.User.Salt, request.User.NameLC);

			Assert.AreEqual(hashed, userWithEmail.Password, "Hashed password");
			Assert.AreEqual(0, repository.List<PasswordResetRequest>().Count, "Number of requests");

		}

		[TestMethod]
		public void SendMessage() {

			var sender = CreateEntry.User(name: "sender");
			var receiver = CreateEntry.User(name: "receiver", email: "test@vocadb.net");
			repository.Save(sender, receiver);
			permissionContext.SetLoggedUser(sender);
			var contract = new UserMessageContract { Sender = new UserForApiContract(sender), Receiver = new UserForApiContract(receiver), Subject = "Subject", Body = "Body" };

			data.SendMessage(contract, string.Empty, string.Empty);

			Assert.AreEqual(1, sender.Messages.Count, "Number of messages for sender");
			Assert.AreEqual(1, receiver.Messages.Count, "Number of messages for receiver");

			var messagesInRepo = repository.List<UserMessage>();
			Assert.AreEqual(2, messagesInRepo.Count, "Number of messages created");

			var sentMessage = messagesInRepo.FirstOrDefault(m => m.Inbox == UserInboxType.Sent);
			Assert.IsNotNull(sentMessage, "Sent message");
			Assert.AreEqual(sender.Messages[0], sentMessage, "Sent message is the same in user collection and repository");
			Assert.AreEqual("Subject", sentMessage.Subject, "sentMessage.Subject");
			Assert.AreEqual(sender, sentMessage.User, "Sent message user is the sender");
			Assert.AreEqual(receiver, sentMessage.Receiver, "sentMessage.Receiver");
			Assert.AreEqual(sender, sentMessage.Sender, "sentMessage.Sender");

			var receivedMessage = messagesInRepo.FirstOrDefault(m => m.Inbox == UserInboxType.Received);
			Assert.IsNotNull(receivedMessage, "Received message");
			Assert.AreEqual(receiver.Messages[0], receivedMessage, "Received message is the same in user collection and repository");
			Assert.AreEqual("Subject", receivedMessage.Subject, "receivedMessage.Subject");
			Assert.AreEqual(receiver, receivedMessage.User, "Received message user is the receiver");
			Assert.AreEqual(receiver, receivedMessage.Receiver, "receivedMessage.Receiver");
			Assert.AreEqual(sender, receivedMessage.Sender, "receivedMessage.Sender");

			Assert.IsNotNull(mailer.Subject, "mailer.Subject");
			Assert.AreEqual("test@vocadb.net", mailer.ToEmail, "mailer.ToEmail");

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void SendMessage_NoPermission() {

			var sender = CreateEntry.User(name: "sender");
			var receiver = CreateEntry.User(name: "receiver");
			repository.Save(sender, receiver);

			var contract = new UserMessageContract { Sender = new UserForApiContract(sender), Receiver = new UserForApiContract(receiver), Subject = "Subject", Body = "Body" };
			data.SendMessage(contract, string.Empty, string.Empty);

		}

		[TestMethod]
		public void UpdateAlbumForUser_Add() {

			var album = repository.Save(CreateEntry.Album());
			data.UpdateAlbumForUser(userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			AssertHasAlbum(userWithEmail, album);

		}

		[TestMethod]
		public void UpdateAlbumForUser_Update() {

			var album = repository.Save(CreateEntry.Album());
			data.UpdateAlbumForUser(userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			data.UpdateAlbumForUser(userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.DigitalDownload, 5);

			var albumForUser = userWithEmail.Albums.First(a => a.Album == album);
			Assert.AreEqual(MediaType.DigitalDownload, albumForUser.MediaType, "Media type was updated");
			Assert.AreEqual(1, userWithEmail.Albums.Count(), "Number of albums for user");
			Assert.AreEqual(1, repository.List<AlbumForUser>().Count, "Number of album links in the repo");

		}

		[TestMethod]
		public void UpdateAlbumForUser_Delete() {

			var album = repository.Save(CreateEntry.Album());
			data.UpdateAlbumForUser(userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			data.UpdateAlbumForUser(userWithEmail.Id, album.Id, PurchaseStatus.Nothing, MediaType.Other, 0);

			Assert.IsFalse(userWithEmail.Albums.Any(a => a.Album == album), "Album was removed");
			Assert.AreEqual(0, userWithEmail.Albums.Count(), "Number of albums for user");
			Assert.AreEqual(0, repository.List<AlbumForUser>().Count, "Number of album links in the repo");

		}

		[TestMethod]
		public void UpdateEventForUser() {

			var releaseEvent = repository.Save(CreateEntry.ReleaseEvent("Miku land"));
			data.UpdateEventForUser(userWithEmail.Id, releaseEvent.Id, UserEventRelationshipType.Attending);

			var link = userWithEmail.Events.FirstOrDefault(e => e.ReleaseEvent == releaseEvent);
			Assert.IsNotNull(link, "Event was added for user");
			Assert.AreEqual(UserEventRelationshipType.Attending, link.RelationshipType, "Link relationship type");

		}

		[TestMethod]
		public void UpdateUser_SetPermissions() {
			
			LoggedUser.GroupId = UserGroupId.Admin;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			contract.AdditionalPermissions = new HashSet<PermissionToken>(new[] { PermissionToken.DesignatedStaff });
			data.UpdateUser(contract);

			var user = repository.Load(contract.Id);
			Assert.IsTrue(user.AdditionalPermissions.Has(PermissionToken.DesignatedStaff), "User has the given permission");

		}

		[TestMethod]
		public void UpdateUser_Name() {

			LoggedUser.GroupId = UserGroupId.Admin;
			permissionContext.RefreshLoggedUser(repository);

			var oldName = userWithoutEmail.Name;
			var contract = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "HatsuneMiku";

			data.UpdateUser(contract);

			var user = repository.Load(contract.Id);
			Assert.AreEqual("HatsuneMiku", user.Name, "Name was updated");
			Assert.AreEqual("hatsunemiku", user.NameLC, "Name was updated");

			var oldNameEntry = repository.List<OldUsername>().FirstOrDefault(u => u.User.Id == userWithoutEmail.Id);
			Assert.IsNotNull(oldNameEntry, "Old name entry was created");
			Assert.AreEqual(oldName, oldNameEntry.OldName, "Old name as expected");

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void UpdateUser_Name_AlreadyInUse() {

			LoggedUser.GroupId = UserGroupId.Admin;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = userWithEmail.Name;

			data.UpdateUser(contract);

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidUserNameException))]
		public void UpdateUser_Name_InvalidCharacters() {

			LoggedUser.GroupId = UserGroupId.Admin;
			permissionContext.RefreshLoggedUser(repository);

			var contract = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "Miku!";

			data.UpdateUser(contract);

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateUser_NotAllowed() {
			
			var contract = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			data.UpdateUser(contract);

		}

		[TestMethod]
		public void UpdateUserSettings_SetEmail() {

			var contract = new UpdateUserSettingsContract(userWithEmail) { Email = "new_email@vocadb.net" };
			userWithEmail.Options.EmailVerified = true;
			var result = data.UpdateUserSettings(contract);

			Assert.IsNotNull(result, "Result");
			var user = GetUserFromRepo(userWithEmail.Name);
			Assert.IsNotNull(user, "User was found in repository");
			Assert.AreEqual("new_email@vocadb.net", user.Email, "Email");
			Assert.IsFalse(user.Options.EmailVerified, "EmailVerified"); // Cancel verification

		}

		[TestMethod]
		public void UpdateUserSettings_Password() {

			var algo = new HMICSHA1PasswordHashAlgorithm();

			var contract = new UpdateUserSettingsContract(userWithEmail) {
				OldPass = "123",
				NewPass = "3939"
			};

			data.UpdateUserSettings(contract);

			Assert.AreEqual(algo.HashPassword("3939", userWithEmail.Salt), userWithEmail.Password, "Password was updated");

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidPasswordException))]
		public void UpdateUserSettings_Password_InvalidOldPassword() {

			var contract = new UpdateUserSettingsContract(userWithEmail) {
				OldPass = "393",
				NewPass = "3939"
			};

			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateUserSettings_NoPermission() {

			data.UpdateUserSettings(new UpdateUserSettingsContract(userWithoutEmail));

		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void UpdateUserSettings_EmailTaken() {

			permissionContext.LoggedUser = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new UpdateUserSettingsContract(userWithoutEmail) { Email = userWithEmail.Email };

			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		public void UpdateUserSettings_EmailTakenButDisabled() {

			userWithEmail.Active = false;
			permissionContext.LoggedUser = new UserWithPermissionsContract(userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new UpdateUserSettingsContract(userWithoutEmail) { Email = userWithEmail.Email };

			data.UpdateUserSettings(contract);

			var user = GetUserFromRepo(userWithoutEmail.Name);
			Assert.IsNotNull(user, "User was found in repository");
			Assert.AreEqual("already_in_use@vocadb.net", user.Email, "Email");

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public void UpdateUserSettings_InvalidEmailFormat() {

			var contract = new UpdateUserSettingsContract(userWithEmail) { Email = "mikumiku" };

			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName() {

			userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(userWithEmail) { Name = "mikumiku" };

			data.UpdateUserSettings(contract);

			Assert.AreEqual("mikumiku", userWithEmail.Name, "Name was changed");
			Assert.AreEqual(1, userWithEmail.OldUsernames.Count, "Old username was added");
			Assert.AreEqual("already_exists", userWithEmail.OldUsernames[0].OldName, "Old name was recorded");

		}

		[TestMethod]
		[ExpectedException(typeof(InvalidUserNameException))]
		public void UpdateUserSettings_ChangeName_Invalid() {

			userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(userWithEmail) { Name = "miku miku" };
			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void UpdateUserSettings_ChangeName_AlreadyInUse() {

			userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(userWithEmail) { Name = userWithoutEmail.Name };
			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		[ExpectedException(typeof(UserNameTooSoonException))]
		public void UpdateUserSettings_ChangeName_TooSoon() {

			userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(39);
			var contract = new UpdateUserSettingsContract(userWithEmail) { Name = "mikumiku" };
			data.UpdateUserSettings(contract);

		}

		[TestMethod]
		public void VerifyEmail() {
			
			Assert.IsFalse(userWithEmail.Options.EmailVerified, "EmailVerified");

			data.VerifyEmail(request.Id);

			Assert.IsTrue(userWithEmail.Options.EmailVerified, "EmailVerified");
			Assert.AreEqual(0, repository.List<PasswordResetRequest>().Count, "Number of requests");

		}

		[TestMethod]
		[ExpectedException(typeof(RequestNotValidException))]
		public void VerifyEmail_DifferentUser() {
			
			request.User = userWithoutEmail;
			data.VerifyEmail(request.Id);

		}

		[TestMethod]
		[ExpectedException(typeof(RequestNotValidException))]
		public void VerifyEmail_DifferentEmail() {
			
			request.Email = "new@vocadb.net";
			data.VerifyEmail(request.Id);

			/*
			Assert.IsTrue(userWithEmail.Options.EmailVerified, "EmailVerified");
			Assert.AreEqual(request.Email, userWithEmail.Email, "Email");*/

		}

	}

}
