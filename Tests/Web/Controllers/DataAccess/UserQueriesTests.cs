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

		private void AssertEqual(User expected, ServerOnlyUserContract actual)
		{
			actual.Should().NotBeNull("Cannot be null");
			actual.Name.Should().Be(expected.Name, "Name");
			actual.Id.Should().Be(expected.Id, "Id");
		}

		private void AssertHasAlbum(User user, Album album)
		{
			_userWithEmail.Albums.Any(a => a.Album == album).Should().BeTrue("User has album");
		}

		private Task<ServerOnlyUserContract> CallCreate(string name = "hatsune_miku", string pass = "3939", string email = "", string hostname = DefaultHostname,
			string culture = DefaultCulture, TimeSpan? timeSpan = null)
		{
			return _data.Create(name, pass, email, hostname, null,
				culture,
				timeSpan ?? TimeSpan.FromMinutes(39), _ipRuleManager, string.Empty);
		}

		private PartialFindResult<ServerOnlyUserContract> CallGetUsers(UserGroupId groupId = UserGroupId.Nothing, string name = null, bool disabled = false, bool verifiedArtists = false, UserSortRule sortRule = UserSortRule.Name, PagingProperties paging = null)
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
			return _data.GetUsers(queryParams, u => new ServerOnlyUserContract(u));
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
			_permissionContext = new FakePermissionContext(new ServerOnlyUserWithPermissionsContract(_userWithEmail, ContentLanguagePreference.Default));
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

			result.IsOk.Should().Be(true, "IsOk");
			AssertEqual(_userWithEmail, result.User);
		}

		[TestMethod]
		public void CheckAuthentication_DifferentCase()
		{
			_userWithEmail.Name = "Already_Exists";
			var result = _data.CheckAuthentication("already_exists", "123", "miku@crypton.jp", DefaultCulture, false);

			result.IsOk.Should().Be(true, "IsOk");
			AssertEqual(_userWithEmail, result.User);
		}

		[TestMethod]
		public void CheckAuthentication_WrongPassword()
		{
			var result = _data.CheckAuthentication("already_exists", "3939", "miku@crypton.jp", DefaultCulture, false);

			result.IsOk.Should().Be(false, "IsOk");
			result.Error.Should().Be(LoginError.InvalidPassword, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_NotFound()
		{
			var result = _data.CheckAuthentication("does_not_exist", "3939", "miku@crypton.jp", DefaultCulture, false);

			result.IsOk.Should().Be(false, "IsOk");
			result.Error.Should().Be(LoginError.NotFound, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_Poisoned()
		{
			_userWithEmail.Options.Poisoned = true;
			var result = _data.CheckAuthentication(_userWithEmail.Name, _userWithEmail.Password, "miku@crypton.jp", DefaultCulture, false);

			result.IsOk.Should().Be(false, "IsOk");
			result.Error.Should().Be(LoginError.AccountPoisoned, "Error");
		}

		[TestMethod]
		public void CheckAuthentication_LoginWithEmail()
		{
			_userWithEmail.Options.EmailVerified = true; // For now, logging in with email is allowed only if the email is verified
			var result = _data.CheckAuthentication(_userWithEmail.Email, "123", "miku@crypton.jp", DefaultCulture, false);

			result.IsOk.Should().Be(true, "IsOk");
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

			_userWithoutEmail.AllAlbums.Count.Should().Be(0, "No albums for user");
			_userWithoutEmail.FavoriteSongs.Count.Should().Be(0, "No songs for user");
			album.UserCollections.Count.Should().Be(0, "Number of users for the album");
			song.UserFavorites.Count.Should().Be(0, "Number of users for the song");
			album.RatingTotal.Should().Be(0, "Album RatingTotal");
			song.RatingScore.Should().Be(0, "Song RatingScore");
		}

		[TestMethod]
		public void ClearRatings_NoPermission()
		{
			_data.Invoking(subject => subject.ClearRatings(_userWithoutEmail.Id)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public async Task Create()
		{
			var name = "hatsune_miku";
			var result = await CallCreate(name: name, email: "mikumiku@crypton.jp");

			result.Should().NotBeNull("Result is not null");
			result.Name.Should().Be(name, "Name");

			var user = GetUserFromRepo(name);
			user.Should().NotBeNull("User found in repository");
			user.Name.Should().Be(name, "Name");
			user.Email.Should().Be("mikumiku@crypton.jp", "Email");
			user.GroupId.Should().Be(UserGroupId.Regular, "GroupId");
			_repository.List<UserReport>().Should().BeEmpty();
			_repository.IsCommitted(user).Should().BeTrue();

			var verificationRequest = _repository.List<PasswordResetRequest>().FirstOrDefault(r => r.User.Equals(user));
			verificationRequest.Should().NotBeNull("Verification request was created");
		}

		[TestMethod]
		public void Create_NameAlreadyExists()
		{
			this.Awaiting(subject => subject.CallCreate(name: "already_exists")).Should().Throw<UserNameAlreadyExistsException>();
		}

		[TestMethod]
		public void Create_NameAlreadyExistsDifferentCase()
		{
			this.Awaiting(subject => subject.CallCreate(name: "Already_Exists")).Should().Throw<UserNameAlreadyExistsException>();
		}

		[TestMethod]
		public void Create_EmailAlreadyExists()
		{
			this.Awaiting(subject => subject.CallCreate(email: "already_in_use@vocadb.net")).Should().Throw<UserEmailAlreadyExistsException>();
		}

		[TestMethod]
		public async Task Create_EmailAlreadyExistsButDisabled()
		{
			_userWithEmail.Active = false;
			var result = await CallCreate(email: "already_in_use@vocadb.net");

			result.Should().NotBeNull("Result is not null");
			result.Name.Should().Be("hatsune_miku", "Name");
		}

		[TestMethod]
		public void Create_InvalidEmailFormat()
		{
			this.Awaiting(subject => subject.CallCreate(email: "mikumiku")).Should().Throw<InvalidEmailFormatException>();
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

			result.Should().NotBeNull("result");
			var report = _repository.List<UserReport>().FirstOrDefault();
			report.Should().NotBeNull("User was reported");
			report.ReportType.Should().Be(UserReportType.MaliciousIP, "Report type");
			report.Hostname.Should().Be(DefaultHostname, "Hostname");

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
		public void Create_RegistrationTimeTrigger()
		{
			this.Awaiting(subject => subject.CallCreate(timeSpan: TimeSpan.FromSeconds(4))).Should().Throw<TooFastRegistrationException>();
			_ipRuleManager.IsAllowed(DefaultHostname).Should().BeTrue("Was not banned");
		}

		[TestMethod]
		public void Create_RegistrationTimeAndBanTrigger()
		{
			this.Awaiting(subject => subject.CallCreate(timeSpan: TimeSpan.FromSeconds(1))).Should().Throw<TooFastRegistrationException>();
			_ipRuleManager.IsAllowed(DefaultHostname).Should().BeFalse("Was banned");
		}

		[TestMethod]
		public void CreateComment()
		{
			var sender = _userWithEmail;
			var receiver = _userWithoutEmail;
			var result = _data.CreateComment(receiver.Id, "Hello world");

			result.Should().NotBeNull("result");
			result.Message.Should().Be("Hello world", "Message");

			var comment = _repository.List<Comment>().FirstOrDefault();
			comment.Should().NotBeNull("Comment was saved");
			comment.Message.Should().Be("Hello world", "Message");
			comment.Author.Id.Should().Be(sender.Id, "Sender Id");
			comment.Entry.Id.Should().Be(receiver.Id, "Receiver Id");

			var notificationMsg = $"{sender.Name} posted a comment on your profile.\n\n{comment.Message}";
			var notification = _repository.List<UserMessage>().FirstOrDefault();
			notification.Should().NotBeNull("Notification was saved");
			notification.Message.Should().Be(notificationMsg, "Notification message");
			notification.Receiver.Id.Should().Be(receiver.Id, "Receiver Id");
		}

		[TestMethod]
		public void CreateTwitter()
		{
			var name = "hatsune_miku";
			var result = _data.CreateTwitter("auth_token", name, "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP");

			result.Should().NotBeNull("Result is not null");
			result.Name.Should().Be(name, "Name");

			var user = GetUserFromRepo(name);
			user.Should().NotBeNull("User found in repository");
			user.Name.Should().Be(name, "Name");
			user.Email.Should().Be("mikumiku@crypton.jp", "Email");
			user.GroupId.Should().Be(UserGroupId.Regular, "GroupId");
			user.Options.LastLoginCulture.CultureCode.Should().Be("ja-JP", "LastLoginCulture");

			user.Options.TwitterOAuthToken.Should().Be("auth_token", "TwitterOAuthToken");
			user.Options.TwitterId.Should().Be(39, "TwitterId");
			user.Options.TwitterName.Should().Be("Miku_Crypton", "TwitterName");
		}

		[TestMethod]
		public void CreateTwitter_NameAlreadyExists()
		{
			_data.Invoking(subject => subject.CreateTwitter("auth_token", "already_exists", "mikumiku@crypton.jp", 39, "Miku_Crypton", "crypton.jp", "ja-JP")).Should().Throw<UserNameAlreadyExistsException>();
		}

		[TestMethod]
		public void CreateTwitter_EmailAlreadyExists()
		{
			_data.Invoking(subject => subject.CreateTwitter("auth_token", "hatsune_miku", "already_in_use@vocadb.net", 39, "Miku_Crypton", "crypton.jp", "ja-JP")).Should().Throw<UserEmailAlreadyExistsException>();
		}

		[TestMethod]
		public void CreateTwitter_InvalidEmailFormat()
		{
			_data.Invoking(subject => subject.CreateTwitter("auth_token", "hatsune_miku", "mikumiku", 39, "Miku_Crypton", "crypton.jp", "ja-JP")).Should().Throw<InvalidEmailFormatException>();
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

			_userWithoutEmail.Active.Should().Be(false, "User was disabled");
		}

		[TestMethod]
		public void DisableUser_NoPermission()
		{
			_data.Invoking(subject => subject.DisableUser(_userWithoutEmail.Id)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void DisableUser_CannotBeDisabled()
		{
			_userWithEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			_userWithoutEmail.AdditionalPermissions.Add(PermissionToken.DisableUsers);
			RefreshLoggedUser();

			_data.Invoking(subject => subject.DisableUser(_userWithoutEmail.Id)).Should().Throw<NotAllowedException>();
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

			result.Length.Should().Be(2, "Number of results");
			var first = result[0];
			first.Item1.Should().Be(electronic.DefaultName, "First result is Electronic");
			first.Item2.Should().Be(3, "Votes for Electronic");

			var second = result[1];
			second.Item1.Should().Be(vocarock.DefaultName, "First result is Vocarock");
			second.Item2.Should().Be(2, "Votes for Vocarock");
		}

		[TestMethod]
		public void GetUsers_NoFilters()
		{
			var result = CallGetUsers();

			result.Should().NotBeNull("result");
			result.Items.Length.Should().Be(2, "Result items");
			result.TotalCount.Should().Be(2, "Total count");
		}

		[TestMethod]
		public void GetUsers_FilterByName()
		{
			var result = CallGetUsers(name: "already");

			result.Should().NotBeNull("result");
			result.Items.Length.Should().Be(1, "Result items");
			result.TotalCount.Should().Be(1, "Total count");
			AssertEqual(_userWithEmail, result.Items.First());
		}

		[TestMethod]
		public void GetUsers_Paging()
		{
			var result = CallGetUsers(paging: new PagingProperties(1, 10, true));
			result.Should().NotBeNull("result");
			result.Items.Length.Should().Be(1, "Result items");
			result.TotalCount.Should().Be(2, "Total count");
			AssertEqual(_userWithoutEmail, result.Items.First());
		}

		[TestMethod]
		public async Task RequestEmailVerification()
		{
			var num = _repository.List<PasswordResetRequest>().Count;

			await _data.RequestEmailVerification(_userWithEmail.Id, string.Empty);

			_mailer.Subject.Should().Be("Verify your email at VocaDB.", "Subject");
			_mailer.ToEmail.Should().Be(_userWithEmail.Email, "ToEmail");
			_repository.List<PasswordResetRequest>().Count.Should().Be(num + 1, "Number of password reset requests");
		}

		[TestMethod]
		public async Task RequestPasswordReset()
		{
			var num = _repository.List<PasswordResetRequest>().Count;

			await _data.RequestPasswordReset(_userWithEmail.Name, _userWithEmail.Email, string.Empty);

			_mailer.Subject.Should().Be("Password reset requested.", "Subject");
			_mailer.ToEmail.Should().Be(_userWithEmail.Email, "ToEmail");
			_repository.List<PasswordResetRequest>().Count.Should().Be(num + 1, "Number of password reset requests");
		}

		[TestMethod]
		public void RequestPasswordReset_NotFound()
		{
			_data.Awaiting(subject => subject.RequestPasswordReset(_userWithEmail.Name, "notfound@vocadb.net", string.Empty)).Should().Throw<UserNotFoundException>();
		}

		[TestMethod]
		public void RequestPasswordReset_Disabled()
		{
			_userWithEmail.Active = false;
			_data.Awaiting(subject => subject.RequestPasswordReset(_userWithEmail.Name, _userWithEmail.Email, string.Empty)).Should().Throw<UserNotFoundException>();
		}

		[TestMethod]
		public void ResetPassword()
		{
			_data.ResetPassword(_request.Id, "123");

			var hashed = PasswordHashAlgorithms.Default.HashPassword("123", _request.User.Salt, _request.User.NameLC);

			_userWithEmail.Password.Should().Be(hashed, "Hashed password");
			_repository.List<PasswordResetRequest>().Count.Should().Be(0, "Number of requests");
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

			sender.Messages.Count.Should().Be(1, "Number of messages for sender");
			receiver.Messages.Count.Should().Be(1, "Number of messages for receiver");

			var messagesInRepo = _repository.List<UserMessage>();
			messagesInRepo.Count.Should().Be(2, "Number of messages created");

			var sentMessage = messagesInRepo.FirstOrDefault(m => m.Inbox == UserInboxType.Sent);
			sentMessage.Should().NotBeNull("Sent message");
			sentMessage.Should().Be(sender.Messages[0], "Sent message is the same in user collection and repository");
			sentMessage.Subject.Should().Be("Subject", "sentMessage.Subject");
			sentMessage.User.Should().Be(sender, "Sent message user is the sender");
			sentMessage.Receiver.Should().Be(receiver, "sentMessage.Receiver");
			sentMessage.Sender.Should().Be(sender, "sentMessage.Sender");

			var receivedMessage = messagesInRepo.FirstOrDefault(m => m.Inbox == UserInboxType.Received);
			receivedMessage.Should().NotBeNull("Received message");
			receivedMessage.Should().Be(receiver.Messages[0], "Received message is the same in user collection and repository");
			receivedMessage.Subject.Should().Be("Subject", "receivedMessage.Subject");
			receivedMessage.User.Should().Be(receiver, "Received message user is the receiver");
			receivedMessage.Receiver.Should().Be(receiver, "receivedMessage.Receiver");
			receivedMessage.Sender.Should().Be(sender, "receivedMessage.Sender");

			_mailer.Subject.Should().NotBeNull("mailer.Subject");
			_mailer.ToEmail.Should().Be("test@vocadb.net", "mailer.ToEmail");
		}

		[TestMethod]
		public void SendMessage_NoPermission()
		{
			var sender = CreateEntry.User(name: "sender");
			var receiver = CreateEntry.User(name: "receiver");
			_repository.Save(sender, receiver);

			var contract = new UserMessageContract { Sender = new UserForApiContract(sender), Receiver = new UserForApiContract(receiver), Subject = "Subject", Body = "Body" };
			_data.Awaiting(subject => subject.SendMessage(contract, string.Empty, string.Empty)).Should().Throw<NotAllowedException>();
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
			albumForUser.MediaType.Should().Be(MediaType.DigitalDownload, "Media type was updated");
			_userWithEmail.Albums.Count().Should().Be(1, "Number of albums for user");
			_repository.List<AlbumForUser>().Count.Should().Be(1, "Number of album links in the repo");
		}

		[TestMethod]
		public void UpdateAlbumForUser_Delete()
		{
			var album = _repository.Save(CreateEntry.Album());
			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Owned, MediaType.PhysicalDisc, 5);

			_data.UpdateAlbumForUser(_userWithEmail.Id, album.Id, PurchaseStatus.Nothing, MediaType.Other, 0);

			_userWithEmail.Albums.Any(a => a.Album == album).Should().BeFalse("Album was removed");
			_userWithEmail.Albums.Count().Should().Be(0, "Number of albums for user");
			_repository.List<AlbumForUser>().Count.Should().Be(0, "Number of album links in the repo");
		}

		[TestMethod]
		public void UpdateEventForUser()
		{
			var releaseEvent = _repository.Save(CreateEntry.ReleaseEvent("Miku land"));
			_data.UpdateEventForUser(_userWithEmail.Id, releaseEvent.Id, UserEventRelationshipType.Attending);

			var link = _userWithEmail.Events.FirstOrDefault(e => e.ReleaseEvent == releaseEvent);
			link.Should().NotBeNull("Event was added for user");
			link.RelationshipType.Should().Be(UserEventRelationshipType.Attending, "Link relationship type");
		}

		[TestMethod]
		public void UpdateUser_SetPermissions()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.AdditionalPermissions = new HashSet<PermissionToken>(new[] { PermissionToken.DesignatedStaff });
			_data.UpdateUser(contract);

			var user = _repository.Load(contract.Id);
			user.AdditionalPermissions.Has(PermissionToken.DesignatedStaff).Should().BeTrue("User has the given permission");
		}

		[TestMethod]
		public void UpdateUser_Name()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var oldName = _userWithoutEmail.Name;
			var contract = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "HatsuneMiku";

			_data.UpdateUser(contract);

			var user = _repository.Load(contract.Id);
			user.Name.Should().Be("HatsuneMiku", "Name was updated");
			user.NameLC.Should().Be("hatsunemiku", "Name was updated");

			var oldNameEntry = _repository.List<OldUsername>().FirstOrDefault(u => u.User.Id == _userWithoutEmail.Id);
			oldNameEntry.Should().NotBeNull("Old name entry was created");
			oldNameEntry.OldName.Should().Be(oldName, "Old name as expected");
		}

		[TestMethod]
		public void UpdateUser_Name_AlreadyInUse()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = _userWithEmail.Name;

			_data.Invoking(subject => subject.UpdateUser(contract)).Should().Throw<UserNameAlreadyExistsException>();
		}

		[TestMethod]
		public void UpdateUser_Name_InvalidCharacters()
		{
			LoggedUser.GroupId = UserGroupId.Admin;
			_permissionContext.RefreshLoggedUser(_repository);

			var contract = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			contract.Name = "Miku!";

			_data.Invoking(subject => subject.UpdateUser(contract)).Should().Throw<InvalidUserNameException>();
		}

		[TestMethod]
		public void UpdateUser_NotAllowed()
		{
			var contract = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			_data.Invoking(subject => subject.UpdateUser(contract)).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void UpdateUserSettings_SetEmail()
		{
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Email = "new_email@vocadb.net" };
			_userWithEmail.Options.EmailVerified = true;
			var result = _data.UpdateUserSettings(contract);

			result.Should().NotBeNull("Result");
			var user = GetUserFromRepo(_userWithEmail.Name);
			user.Should().NotBeNull("User was found in repository");
			user.Email.Should().Be("new_email@vocadb.net", "Email");
			user.Options.EmailVerified.Should().BeFalse("EmailVerified"); // Cancel verification
		}

		[TestMethod]
		public void UpdateUserSettings_Password()
		{
			var algo = new HMICSHA1PasswordHashAlgorithm();

			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail)
			{
				OldPass = "123",
				NewPass = "3939"
			};

			_data.UpdateUserSettings(contract);

			_userWithEmail.Password.Should().Be(algo.HashPassword("3939", _userWithEmail.Salt), "Password was updated");
		}

		[TestMethod]
		public void UpdateUserSettings_Password_InvalidOldPassword()
		{
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail)
			{
				OldPass = "393",
				NewPass = "3939"
			};

			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<InvalidPasswordException>();
		}

		[TestMethod]
		public void UpdateUserSettings_NoPermission()
		{
			_data.Invoking(subject => subject.UpdateUserSettings(new ServerOnlyUpdateUserSettingsContract(_userWithoutEmail))).Should().Throw<NotAllowedException>();
		}

		[TestMethod]
		public void UpdateUserSettings_EmailTaken()
		{
			_permissionContext.LoggedUser = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithoutEmail) { Email = _userWithEmail.Email };

			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<UserEmailAlreadyExistsException>();
		}

		[TestMethod]
		public void UpdateUserSettings_EmailTakenButDisabled()
		{
			_userWithEmail.Active = false;
			_permissionContext.LoggedUser = new ServerOnlyUserWithPermissionsContract(_userWithoutEmail, ContentLanguagePreference.Default);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithoutEmail) { Email = _userWithEmail.Email };

			_data.UpdateUserSettings(contract);

			var user = GetUserFromRepo(_userWithoutEmail.Name);
			user.Should().NotBeNull("User was found in repository");
			user.Email.Should().Be("already_in_use@vocadb.net", "Email");
		}

		[TestMethod]
		public void UpdateUserSettings_InvalidEmailFormat()
		{
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Email = "mikumiku" };

			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<InvalidEmailFormatException>();
		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Name = "mikumiku" };

			_data.UpdateUserSettings(contract);

			_userWithEmail.Name.Should().Be("mikumiku", "Name was changed");
			_userWithEmail.OldUsernames.Count.Should().Be(1, "Old username was added");
			_userWithEmail.OldUsernames[0].OldName.Should().Be("already_exists", "Old name was recorded");
		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName_Invalid()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Name = "miku miku" };
			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<InvalidUserNameException>();
		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName_AlreadyInUse()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(720);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Name = _userWithoutEmail.Name };
			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<UserNameAlreadyExistsException>();
		}

		[TestMethod]
		public void UpdateUserSettings_ChangeName_TooSoon()
		{
			_userWithEmail.CreateDate = DateTime.Now - TimeSpan.FromDays(39);
			var contract = new ServerOnlyUpdateUserSettingsContract(_userWithEmail) { Name = "mikumiku" };
			_data.Invoking(subject => subject.UpdateUserSettings(contract)).Should().Throw<UserNameTooSoonException>();
		}

		[TestMethod]
		public void VerifyEmail()
		{
			_userWithEmail.Options.EmailVerified.Should().BeFalse("EmailVerified");

			_data.VerifyEmail(_request.Id);

			_userWithEmail.Options.EmailVerified.Should().BeTrue("EmailVerified");
			_repository.List<PasswordResetRequest>().Count.Should().Be(0, "Number of requests");
		}

		[TestMethod]
		public void VerifyEmail_DifferentUser()
		{
			_request.User = _userWithoutEmail;
			_data.Invoking(subject => subject.VerifyEmail(_request.Id)).Should().Throw<RequestNotValidException>();
		}

		[TestMethod]
		public void VerifyEmail_DifferentEmail()
		{
			_request.Email = "new@vocadb.net";
			_data.Invoking(subject => subject.VerifyEmail(_request.Id)).Should().Throw<RequestNotValidException>();

			/*
			userWithEmail.Options.EmailVerified.Should().BeTrue("EmailVerified");
			userWithEmail.Email.Should().Be(request.Email, "Email");*/
		}
	}
}
