#nullable disable

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
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
using VocaDb.Model.Utils.Config;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Web.Controllers.DataAccess
{
	/// <summary>
	/// Tests for <see cref="UserQueries"/>.
	/// </summary>
	[TestClass]
	public class UserQueriesTests
	{
		private const string DefaultCulture = "ja-JP";
		private const string DefaultHostname = "crypton.jp";
		private UserQueries _data;
		private FakeUserMessageMailer _mailer;
		private PasswordResetRequest _request;
		private FakePermissionContext _permissionContext;
		private FakeUserRepository _repository;
		private readonly IPRuleManager _ipRuleManager = new();
		private FakeStopForumSpamClient _stopForumSpamClient;
		private User _userWithEmail;
		private User _userWithoutEmail;

		private User LoggedUser => _userWithEmail;

		private void AssertEqual(User expected, UserContract actual)
		{
			Assert.IsNotNull(actual, "Cannot be null");
			Assert.AreEqual(expected.Name, actual.Name, "Name");
			Assert.AreEqual(expected.Id, actual.Id, "Id");
		}

		private void AssertHasAlbum(User user, Album album)
		{
			Assert.IsTrue(_userWithEmail.Albums.Any(a => a.Album == album), "User has album");
		}

		private Task<UserContract> CallCreate(string name = "hatsune_miku", string pass = "3939", string email = "", string hostname = DefaultHostname,
			string culture = DefaultCulture, TimeSpan? timeSpan = null)
		{
			return _data.Create(name, pass, email, hostname, null,
				culture,
				timeSpan ?? TimeSpan.FromMinutes(39), _ipRuleManager, string.Empty);
		}

		private PartialFindResult<UserContract> CallGetUsers(UserGroupId groupId = UserGroupId.Nothing, string name = null, bool disabled = false, bool verifiedArtists = false, UserSortRule sortRule = UserSortRule.Name, PagingProperties paging = null)
		{
			var queryParams = new UserQueryParams
			{
				Common = new CommonSearchParams(SearchTextQuery.Create(name), false, false),
				Group = groupId,
				IncludeDisabled = disabled,
				OnlyVerifiedArtists = verifiedArtists,
				Sort = sortRule,
				Paging = paging ?? new PagingProperties(0, 10, true)
			};
			return _data.GetUsers(queryParams, u => new UserContract(u));
		}

		private User GetUserFromRepo(string username)
		{
			return _repository.List<User>().FirstOrDefault(u => u.Name == username);
		}

		private void RefreshLoggedUser()
		{
			_permissionContext.RefreshLoggedUser(_repository);
		}

		[TestInitialize]
		public void SetUp()
		{
			_userWithEmail = new User("already_exists", "123", "already_in_use@vocadb.net", PasswordHashAlgorithms.Default) { Id = 123 };
			_userWithoutEmail = new User("no_email", "222", string.Empty, PasswordHashAlgorithms.Default) { Id = 321 };
			_repository = new FakeUserRepository(_userWithEmail, _userWithoutEmail);
			_repository.Add(_userWithEmail.Options);
			_permissionContext = new FakePermissionContext(new UserWithPermissionsContract(_userWithEmail, ContentLanguagePreference.Default));
			_stopForumSpamClient = new FakeStopForumSpamClient();
			_mailer = new FakeUserMessageMailer();
			_data = new UserQueries(_repository, _permissionContext, new FakeEntryLinkFactory(), _stopForumSpamClient, _mailer,
				new FakeUserIconFactory(), new InMemoryImagePersister(), new FakeObjectCache(), new Model.Service.BrandableStrings.BrandableStringsManager(new VdbConfigManager()), new EnumTranslations());

			_request = new PasswordResetRequest(_userWithEmail) { Id = Guid.NewGuid() };
			_repository.Add(_request);
		}

		[TestMethod]
		public void CheckAuthentication()
		{
			var result = _data.CheckAuthentication("already_exists", "123", "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(_userWithEmail, result.User);
		}

		[TestMethod]
		public void CheckAuthentication_DifferentCase()
		{
			_userWithEmail.Name = "Already_Exists";
			var result = _data.CheckAuthentication("already_exists", "123", "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(_userWithEmail, result.User);
		}

		[TestMethod]
		public void CheckAuthentication_WrongPassword()
		{
			var result = _data.CheckAuthentication("already_exists", "3939", "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.InvalidPassword, result.Error, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_NotFound()
		{
			var result = _data.CheckAuthentication("does_not_exist", "3939", "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.NotFound, result.Error, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_Poisoned()
		{
			_userWithEmail.Options.Poisoned = true;
			var result = _data.CheckAuthentication(_userWithEmail.Name, _userWithEmail.Password, "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(false, result.IsOk, "IsOk");
			Assert.AreEqual(LoginError.AccountPoisoned, result.Error, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_LoginWithEmail()
		{
			_userWithEmail.Options.EmailVerified = true; // For now, logging in with email is allowed only if the email is verified
			var result = _data.CheckAuthentication(_userWithEmail.Email, "123", "miku@crypton.jp", DefaultCulture, false);

			Assert.AreEqual(true, result.IsOk, "IsOk");
			AssertEqual(_userWithEmail, result.User);
		}

		[TestMethod]
		public void ClearRatings()
		{
			_userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();
			var album = CreateEntry.Album();
			var song = CreateEntry.Song();
			_repository.Save(album);
			_repository.Save(song);
			_repository.Save(_userWithoutEmail.AddAlbum(album, PurchaseStatus.Nothing, MediaType.DigitalDownload, 5));
			_repository.Save(_userWithoutEmail.AddSongToFavorites(song, SongVoteRating.Favorite));

			_data.ClearRatings(_userWithoutEmail.Id);

			Assert.AreEqual(0, _userWithoutEmail.AllAlbums.Count, "No albums for user");
			Assert.AreEqual(0, _userWithoutEmail.FavoriteSongs.Count, "No songs for user");
			Assert.AreEqual(0, album.UserCollections.Count, "Number of users for the album");
			Assert.AreEqual(0, song.UserFavorites.Count, "Number of users for the song");
			Assert.AreEqual(0, album.RatingTotal, "Album RatingTotal");
			Assert.AreEqual(0, song.RatingScore, "Song RatingScore");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void ClearRatings_NoPermission()
		{
			_data.ClearRatings(_userWithoutEmail.Id);
		}

		[TestMethod]
		public async Task Create()
		{
			var name = "hatsune_miku";
			var result = await CallCreate(name: name, email: "mikumiku@crypton.jp");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual(name, result.Name, "Name");

			var user = GetUserFromRepo(name);
			Assert.IsNotNull(user, "User found in repository");
			Assert.AreEqual(name, user.Name, "Name");
			Assert.AreEqual("mikumiku@crypton.jp", user.Email, "Email");
			Assert.AreEqual(UserGroupId.Regular, user.GroupId, "GroupId");
			_repository.List<UserReport>().Should().BeEmpty();
			_repository.IsCommitted(user).Should().BeTrue();

			var verificationRequest = _repository.List<PasswordResetRequest>().FirstOrDefault(r => r.User.Equals(user));
			Assert.IsNotNull(verificationRequest, "Verification request was created");
		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public async Task Create_NameAlreadyExists()
		{
			await CallCreate(name: "already_exists");
		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public async Task Create_NameAlreadyExistsDifferentCase()
		{
			await CallCreate(name: "Already_Exists");
		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public async Task Create_EmailAlreadyExists()
		{
			await CallCreate(email: "already_in_use@vocadb.net");
		}

		[TestMethod]
		public async Task Create_EmailAlreadyExistsButDisabled()
		{
			_userWithEmail.Active = false;
			var result = await CallCreate(email: "already_in_use@vocadb.net");

			Assert.IsNotNull(result, "Result is not null");
			Assert.AreEqual("hatsune_miku", result.Name, "Name");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public async Task Create_InvalidEmailFormat()
		{
			await CallCreate(email: "mikumiku");
		}

		[TestMethod]
		public async Task Create_FlaggedUser_Reported()
		{
			_stopForumSpamClient.Response = new SFSResponseContract { Appears = true, Confidence = 30d, Frequency = 50 };
			var result = await CallCreate();

			result.Should().NotBeNull();
			var report = _repository.List<UserReport>().FirstOrDefault();
			report.Should().NotBeNull(because: "User was reported");
			report.ReportType.Should().Be(UserReportType.MaliciousIP);
			report.Hostname.Should().Be(DefaultHostname);

			var user = GetUserFromRepo(result.Name);
			user.GroupId.Should().Be(UserGroupId.Regular, because: "User is not limited");
			_repository.IsCommitted(user).Should().BeTrue();
		}

		[TestMethod]
		public async Task Create_FlaggedUser_NotReported()
		{
			_stopForumSpamClient.Response = new SFSResponseContract { Appears = true, Confidence = 0.5d, Frequency = 1 };
			var result = await CallCreate();

			result.Should().NotBeNull();
			_repository.List<UserReport>().Should().BeEmpty(because: "Confidence too low");
		}

		[TestMethod]
		public async Task Create_LikelyMaliciousIP_Limited()
		{
			_stopForumSpamClient.Response = new SFSResponseContract { Appears = true, Confidence = 60d, Frequency = 100 };
			var result = await CallCreate();

			Assert.IsNotNull(result, "result");
			var report = _repository.List<UserReport>().FirstOrDefault();
			Assert.IsNotNull(report, "User was reported");
			Assert.AreEqual(UserReportType.MaliciousIP, report.ReportType, "Report type");
			Assert.AreEqual(DefaultHostname, report.Hostname, "Hostname");

			var user = GetUserFromRepo(result.Name);
			user.GroupId.Should().Be(UserGroupId.Limited, because: "User was limited");
			_repository.IsCommitted(user).Should().BeTrue();
		}

		[TestMethod]
		public void Create_MalicousIP_Banned()
		{
			_stopForumSpamClient.Response = new SFSResponseContract { Appears = true, Confidence = 99d, Frequency = 100 };
			this.Invoking(self => self.CallCreate()).Should().Throw<RestrictedIPException>("User is malicious");
			_ipRuleManager.PermBannedIPs.Contains(DefaultHostname).Should().BeTrue("User was banned");

			_repository.List<UserReport>().Should().BeEmpty("Report was not created");

			var ipRule = _repository.List<IPRule>().Should().Contain(rule => rule.Address == DefaultHostname).Subject;
			_repository.IsCommitted(ipRule).Should().BeTrue("IPRule was committed despite exception");
		}

		[TestMethod]
		[ExpectedException(typeof(TooFastRegistrationException))]
		public async Task Create_RegistrationTimeTrigger()
		{
			await CallCreate(timeSpan: TimeSpan.FromSeconds(4));
			Assert.IsTrue(_ipRuleManager.IsAllowed(DefaultHostname), "Was not banned");
		}

		[TestMethod]
		[ExpectedException(typeof(TooFastRegistrationException))]
		public async Task Create_RegistrationTimeAndBanTrigger()
		{
			await CallCreate(timeSpan: TimeSpan.FromSeconds(1));
			Assert.IsFalse(_ipRuleManager.IsAllowed(DefaultHostname), "Was banned");
		}

		[TestMethod]
		public void CreateComment()
		{
			var sender = _userWithEmail;
			var receiver = _userWithoutEmail;
			var result = _data.CreateComment(receiver.Id, "Hello world");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual("Hello world", result.Message, "Message");

			var comment = _repository.List<Comment>().FirstOrDefault();
			Assert.IsNotNull(comment, "Comment was saved");
			Assert.AreEqual("Hello world", comment.Message, "Message");
			Assert.AreEqual(sender.Id, comment.Author.Id, "Sender Id");
			Assert.AreEqual(receiver.Id, comment.Entry.Id, "Receiver Id");

			var notificationMsg = $"{sender.Name} posted a comment on your profile.\n\n{comment.Message}";
			var notification = _repository.List<UserMessage>().FirstOrDefault();
			Assert.IsNotNull(notification, "Notification was saved");
			Assert.AreEqual(notificationMsg, notification.Message, "Notification message");
			Assert.AreEqual(receiver.Id, notification.Receiver.Id, "Receiver Id");
		}

		[TestMethod]
		public void CreateTwitter()
		{
			var name = "hatsune_miku";
			var result = _data.CreateTwitter("auth_token", name, "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

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
		public void CreateTwitter_NameAlreadyExists()
		{
			_data.CreateTwitter("auth_token", "already_exists", "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP");
		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void CreateTwitter_EmailAlreadyExists()
		{
			_data.CreateTwitter("auth_token", "hatsune_miku", "already_in_use@vocadb.net", 39, "Miku_Crypton", "crypton.jp", "ja-JP");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public void CreateTwitter_InvalidEmailFormat()
		{
			_data.CreateTwitter("auth_token", "hatsune_miku", "mikumiku", 39, "Miku_Crypton", "crypton.jp", "ja-JP");
		}

		[TestMethod]
		public void CreateReport()
		{
			var user = _repository.Save(CreateEntry.User());

			_data.CreateReport(user.Id, UserReportType.Spamming, "mikumiku", "Too much negis!");

			_repository.List<UserReport>().Should().Contain(rep => rep.Entry.Id == user.Id && rep.User.Id == _userWithEmail.Id);
			user.GroupId.Should().Be(UserGroupId.Regular);
			user.Active.Should().BeTrue();
		}

		[TestMethod]
		public void CreateReport_Limited()
		{
			var user = _repository.Save(CreateEntry.User());

			for (int i = 0; i < 2; ++i)
			{
				var reporter = _repository.Save(CreateEntry.User());
				_permissionContext.SetLoggedUser(reporter);
				_data.CreateReport(user.Id, UserReportType.Spamming, "mikumiku", "Too much negis!", reportCountLimit: 2, reportCountDisable: 3);
			}

			user.GroupId.Should().Be(UserGroupId.Limited);
			_repository.List<UserReport>().Should().HaveCount(2);
		}

		[TestMethod]
		public void CreateReport_Disabled()
		{
			var user = _repository.Save(CreateEntry.User());

			for (int i = 0; i < 3; ++i)
			{
				var reporter = _repository.Save(CreateEntry.User());
				_permissionContext.SetLoggedUser(reporter);
				_data.CreateReport(user.Id, UserReportType.Spamming, "mikumiku", "Too much negis!", reportCountLimit: 2, reportCountDisable: 3);
			}

			user.Active.Should().BeFalse();
			_repository.List<UserReport>().Should().HaveCount(3);
		}

		[TestMethod]
		public void CreateReport_IgnoreDuplicates()
		{
			var user = _repository.Save(CreateEntry.User());

			for (int i = 0; i < 3; ++i)
			{
				_data.CreateReport(user.Id, UserReportType.Spamming, "mikumiku", "Too much negis!", reportCountLimit: 2, reportCountDisable: 3);
			}

			user.GroupId.Should().Be(UserGroupId.Regular);
			user.Active.Should().BeTrue();
			_repository.List<UserReport>().Should().HaveCount(1);
		}

		[TestMethod]
		public void DisableUser()
		{
			_userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();

			_data.DisableUser(_userWithoutEmail.Id);

			Assert.AreEqual(false, _userWithoutEmail.Active, "User was disabled");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void DisableUser_NoPermission()
		{
			_data.DisableUser(_userWithoutEmail.Id);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void DisableUser_CannotBeDisabled()
		{
			_userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			_userWithoutEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();

			_data.DisableUser(_userWithoutEmail.Id);
		}

		[TestMethod]
		public void GetRatingsByGenre()
		{
			var fakeTagMock = new Mock<Tag>();
			var fakeTag = fakeTagMock.Object;
			var vocarock = new Tag("Vocarock", TagCommonCategoryNames.Genres) { Parent = fakeTag };
			var electronic = new Tag("Electronic", TagCommonCategoryNames.Genres) { Parent = fakeTag };
			var trance = new Tag("Trance", TagCommonCategoryNames.Genres) { Parent = electronic };
			_repository.Save(vocarock, electronic, trance);
			_repository.SaveNames(vocarock, electronic, trance);

			var song1 = CreateEntry.Song(name: "Nebula");
			var song2 = CreateEntry.Song(name: "Anger");
			var song3 = CreateEntry.Song(name: "DYE");
			_repository.Add(song1, song2, song3);

			_userWithEmail.AddSongToFavorites(song1, SongVoteRating.Favorite);
			_userWithEmail.AddSongToFavorites(song2, SongVoteRating.Favorite);
			_userWithEmail.AddSongToFavorites(song3, SongVoteRating.Favorite);

			var usage1 = CreateEntry.SongTagUsage(song1, vocarock, _userWithEmail);
			var usage2 = CreateEntry.SongTagUsage(song1, trance, _userWithEmail);
			var usage3 = CreateEntry.SongTagUsage(song2, vocarock, _userWithEmail);
			var usage4 = CreateEntry.SongTagUsage(song2, trance, _userWithEmail);
			var usage5 = CreateEntry.SongTagUsage(song3, trance, _userWithEmail);
			_repository.Add(usage1, usage2, usage3, usage4, usage5);

			var result = _data.GetRatingsByGenre(_userWithEmail.Id);

			Assert.AreEqual(2, result.Length, "Number of results");
			var first = result[0];
			Assert.AreEqual(electronic.DefaultName, first.Item1, "First result is Electronic");
			Assert.AreEqual(3, first.Item2, "Votes for Electronic");

			var second = result[1];
			Assert.AreEqual(vocarock.DefaultName, second.Item1, "First result is Vocarock");
			Assert.AreEqual(2, second.Item2, "Votes for Vocarock");
		}

		[TestMethod]
		public void GetUsers_NoFilters()
		{
			var result = CallGetUsers();

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(2, result.Items.Length, "Result items");
			Assert.AreEqual(2, result.TotalCount, "Total count");
		}

		[TestMethod]
		public void GetUsers_FilterByName()
		{
			var result = CallGetUsers(name: "already");

			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Items.Length, "Result items");
			Assert.AreEqual(1, result.TotalCount, "Total count");
			AssertEqual(_userWithEmail, result.Items.First());
		}

		[TestMethod]
		public void GetUsers_Paging()
		{
			var result = CallGetUsers(paging: new PagingProperties(1, 10, true));
			Assert.IsNotNull(result, "result");
			Assert.AreEqual(1, result.Items.Length, "Result items");
			Assert.AreEqual(2, result.TotalCount, "Total count");
			AssertEqual(_userWithoutEmail, result.Items.First());
		}

		[TestMethod]
		public async Task RequestEmailVerification()
		{
			var num = _repository.List<PasswordResetRequest>().Count;

			await _data.RequestEmailVerification(_userWithEmail.Id, string.Empty);

			Assert.AreEqual("Verify your email at VocaDB.", _mailer.Subject, "Subject");
			Assert.AreEqual(_userWithEmail.Email, _mailer.ToEmail, "ToEmail");
			Assert.AreEqual(num + 1, _repository.List<PasswordResetRequest>().Count, "Number of password reset requests");
		}

		[TestMethod]
		public async Task RequestPasswordReset()
		{
			var num = _repository.List<PasswordResetRequest>().Count;

			await _data.RequestPasswordReset(_userWithEmail.Name, _userWithEmail.Email, string.Empty);

			Assert.AreEqual("Password reset requested.", _mailer.Subject, "Subject");
			Assert.AreEqual(_userWithEmail.Email, _mailer.ToEmail, "ToEmail");
			Assert.AreEqual(num + 1, _repository.List<PasswordResetRequest>().Count, "Number of password reset requests");
		}

		[TestMethod]
		[ExpectedException(typeof(UserNotFoundException))]
		public async Task RequestPasswordReset_NotFound()
		{
			await _data.RequestPasswordReset(_userWithEmail.Name, "notfound@vocadb.net", string.Empty);
		}

		[TestMethod]
		[ExpectedException(typeof(UserNotFoundException))]
		public async Task RequestPasswordReset_Disabled()
		{
			_userWithEmail.Active = false;
			await _data.RequestPasswordReset(_userWithEmail.Name, _userWithEmail.Email, string.Empty);
		}

		[TestMethod]
		public void ResetPassword()
		{
			_data.ResetPassword(_request.Id, "123");

			var hashed = PasswordHashAlgorithms.Default.HashPassword("123", _request.User.Salt, _request.User.NameLC);

			Assert.AreEqual(hashed, _userWithEmail.Password, "Hashed password");
			Assert.AreEqual(0, _repository.List<PasswordResetRequest>().Count, "Number of requests");
		}

		[TestMethod]
		public async Task SendMessage()
		{
			var sender = CreateEntry.User(name: "sender");
			var receiver = CreateEntry.User(name: "receiver", email: "test@vocadb.net");
			_repository.Save(sender, receiver);
			_permissionContext.SetLoggedUser(sender);
			var contract = new UserMessageContract { Sender = new UserForApiContract(sender), Receiver = new UserForApiContract(receiver), Subject = "Subject", Body = "Body" };

			await _data.SendMessage(contract, string.Empty, string.Empty);

			Assert.AreEqual(1, sender.Messages.Count, "Number of messages for sender");
			Assert.AreEqual(1, receiver.Messages.Count, "Number of messages for receiver");

			var messagesInRepo = _repository.List<UserMessage>();
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

			Assert.IsNotNull(_mailer.Subject, "mailer.Subject");
			Assert.AreEqual("test@vocadb.net", _mailer.ToEmail, "mailer.ToEmail");
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public async Task SendMessage_NoPermission()
		{
			var sender = CreateEntry.User(name: "sender");
			var receiver = CreateEntry.User(name: "receiver");
			_repository.Save(sender, receiver);

			var contract = new UserMessageContract { Sender = new UserForApiContract(sender), Receiver = new UserForApiContract(receiver), Subject = "Subject", Body = "Body" };
			await _data.SendMessage(contract, string.Empty, string.Empty);
		}

		[TestMethod]
		public void UpdateAlbumForUser_Add()
		{
			var album = _repository.Save(CreateEntry.Album());
			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			AssertHasAlbum(_userWithEmail, album);
		}

		[TestMethod]
		public void UpdateAlbumForUser_Update()
		{
			var album = _repository.Save(CreateEntry.Album());
			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.DigitalDownload, 5);

			var albumForUser = _userWithEmail.Albums.First(a => a.Album == album);
			Assert.AreEqual(MediaType.DigitalDownload, albumForUser.MediaType, "Media type was updated");
			Assert.AreEqual(1, _userWithEmail.Albums.Count(), "Number of albums for user");
			Assert.AreEqual(1, _repository.List<AlbumForUser>().Count, "Number of album links in the repo");
		}

		[TestMethod]
		public void UpdateAlbumForUser_Delete()
		{
			var album = _repository.Save(CreateEntry.Album());
			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Nothing, MediaType.Other, 0);

			Assert.IsFalse(_userWithEmail.Albums.Any(a => a.Album == album), "Album was removed");
			Assert.AreEqual(0, _userWithEmail.Albums.Count(), "Number of albums for user");
			Assert.AreEqual(0, _repository.List<AlbumForUser>().Count, "Number of album links in the repo");
		}

		[TestMethod]
		public void UpdateEventForUser()
		{
			var releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("Miku land"));
			_data.UpdateEventForUser(_userWithEmail.Id, releaseEvent.Id, UserEventRelationshipType.Attending);

			var link = _userWithEmail.Events.FirstOrDefault(e => e.ReleaseEvent == releaseEvent);
			Assert.IsNotNull(link, "Event was added for user");
			Assert.AreEqual(UserEventRelationshipType.Attending, link.RelationshipType, "Link relationship type");
		}

		[TestMethod]
		public void UpdateUser_SetPermissions()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.AdditionalPermissions = new HashSet<PermissionToken>(new[] { PermissionToken.DesignatedStaff });
			_data.UpdateUser(contract);

			var user = _repository.Load(contract.Id);
			Assert.IsTrue(user.AdditionalPermissions.Has(PermissionToken.DesignatedStaff), "User has the given permission");
		}

		[TestMethod]
		public void UpdateUser_Name()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var oldName = _userWithoutEmail.Name;
			var contract = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "HatsuneMiku";

			_data.UpdateUser(contract);

			var user = _repository.Load(contract.Id);
			Assert.AreEqual("HatsuneMiku", user.Name, "Name was updated");
			Assert.AreEqual("hatsunemiku", user.NameLC, "Name was updated");

			var oldNameEntry = _repository.List<OldUsername>().FirstOrDefault(u => u.User.Id == _userWithoutEmail.Id);
			Assert.IsNotNull(oldNameEntry, "Old name entry was created");
			Assert.AreEqual(oldName, oldNameEntry.OldName, "Old name as expected");
		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void UpdateUser_Name_AlreadyInUse()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = _userWithEmail.Name;

			_data.UpdateUser(contract);
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidUserNameException))]
		public void UpdateUser_Name_InvalidCharacters()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "Miku!";

			_data.UpdateUser(contract);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateUser_NotAllowed()
		{
			var contract = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			_data.UpdateUser(contract);
		}

		[TestMethod]
		public void UpdateUserSettings_SetEmail()
		{
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Email = "new_email@vocadb.net" };
			_userWithEmail.Options.EmailVerified = true;
			var result = _data.UpdateUserSettings(contract);

			Assert.IsNotNull(result, "Result");
			var user = GetUserFromRepo(_userWithEmail.Name);
			Assert.IsNotNull(user, "User was found in repository");
			Assert.AreEqual("new_email@vocadb.net", user.Email, "Email");
			Assert.IsFalse(user.Options.EmailVerified, "EmailVerified"); // Cancel verification
		}

		[TestMethod]
		public void UpdateUserSettings_Password()
		{
			var algo = new HMICSHA1PasswordHashAlgorithm();

			var contract = new UpdateUserSettingsContract(_userWithEmail)
			{
				OldPass = "123",
				NewPass = "3939"
			};

			_data.UpdateUserSettings(contract);

			Assert.AreEqual(algo.HashPassword("3939", _userWithEmail.Salt), _userWithEmail.Password, "Password was updated");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidPasswordException))]
		public void UpdateUserSettings_Password_InvalidOldPassword()
		{
			var contract = new UpdateUserSettingsContract(_userWithEmail)
			{
				OldPass = "393",
				NewPass = "3939"
			};

			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		[ExpectedException(typeof(NotAllowedException))]
		public void UpdateUserSettings_NoPermission()
		{
			_data.UpdateUserSettings(new UpdateUserSettingsContract(_userWithoutEmail));
		}

		[TestMethod]
		[ExpectedException(typeof(UserEmailAlreadyExistsException))]
		public void UpdateUserSettings_EmailTaken()
		{
			_permissionContext.LoggedUser = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new UpdateUserSettingsContract(_userWithoutEmail) { Email = _userWithEmail.Email };

			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		public void UpdateUserSettings_EmailTakenButDisabled()
		{
			_userWithEmail.Active = false;
			_permissionContext.LoggedUser = new UserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new UpdateUserSettingsContract(_userWithoutEmail) { Email = _userWithEmail.Email };

			_data.UpdateUserSettings(contract);

			var user = GetUserFromRepo(_userWithoutEmail.Name);
			Assert.IsNotNull(user, "User was found in repository");
			Assert.AreEqual("already_in_use@vocadb.net", user.Email, "Email");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidEmailFormatException))]
		public void UpdateUserSettings_InvalidEmailFormat()
		{
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Email = "mikumiku" };

			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Name = "mikumiku" };

			_data.UpdateUserSettings(contract);

			Assert.AreEqual("mikumiku", _userWithEmail.Name, "Name was changed");
			Assert.AreEqual(1, _userWithEmail.OldUsernames.Count, "Old username was added");
			Assert.AreEqual("already_exists", _userWithEmail.OldUsernames[0].OldName, "Old name was recorded");
		}

		[TestMethod]
		[ExpectedException(typeof(InvalidUserNameException))]
		public void UpdateUserSettings_ChangeName_Invalid()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Name = "miku miku" };
			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		[ExpectedException(typeof(UserNameAlreadyExistsException))]
		public void UpdateUserSettings_ChangeName_AlreadyInUse()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Name = _userWithoutEmail.Name };
			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		[ExpectedException(typeof(UserNameTooSoonException))]
		public void UpdateUserSettings_ChangeName_TooSoon()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(39);
			var contract = new UpdateUserSettingsContract(_userWithEmail) { Name = "mikumiku" };
			_data.UpdateUserSettings(contract);
		}

		[TestMethod]
		public void VerifyEmail()
		{
			Assert.IsFalse(_userWithEmail.Options.EmailVerified, "EmailVerified");

			_data.VerifyEmail(_request.Id);

			Assert.IsTrue(_userWithEmail.Options.EmailVerified, "EmailVerified");
			Assert.AreEqual(0, _repository.List<PasswordResetRequest>().Count, "Number of requests");
		}

		[TestMethod]
		[ExpectedException(typeof(RequestNotValidException))]
		public void VerifyEmail_DifferentUser()
		{
			_request.User = _userWithoutEmail;
			_data.VerifyEmail(_request.Id);
		}

		[TestMethod]
		[ExpectedException(typeof(RequestNotValidException))]
		public void VerifyEmail_DifferentEmail()
		{
			_request.Email = "new@vocadb.net";
			_data.VerifyEmail(_request.Id);

			/*
			Assert.IsTrue(userWithEmail.Options.EmailVerified, "EmailVerified");
			Assert.AreEqual(request.Email, userWithEmail.Email, "Email");*/
		}
	}
}
