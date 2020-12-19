#nullable disable

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.Security;
using VocaDb.Tests.TestSupport;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Tests.Service.Helpers
{
	/// <summary>
	/// Tests for <see cref="FollowedArtistNotifier"/>
	/// </summary>
	[TestClass]
	public class FollowedArtistNotifierTests
	{
		private Album _album;
		private User _creator;
		private FakeEntryLinkFactory _entryLinkFactory;
		private FakeUserMessageMailer _mailer;
		private Artist _producer;
		private FakeRepository<UserMessage> _repository;
		private User _user;
		private Artist _vocalist;

		private Task CallSendNotifications(IUser creator)
		{
			return _repository.HandleTransactionAsync(ctx =>
			{
				return new FollowedArtistNotifier(_entryLinkFactory, _mailer, new EnumTranslations(), new EntrySubTypeNameFactory())
					.SendNotificationsAsync(ctx, _album, new[] { _producer, _vocalist }, creator);
			});
		}

		private T Save<T>(T entry) where T : class, IDatabaseObject
		{
			return _repository.Save(entry);
		}

		[TestInitialize]
		public void SetUp()
		{
			_entryLinkFactory = new FakeEntryLinkFactory();
			_repository = new FakeRepository<UserMessage>();
			_mailer = new FakeUserMessageMailer();

			_album = Save(new Album(new LocalizedString("New Album", ContentLanguageSelection.English)));
			_producer = Save(new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer });
			_vocalist = Save(new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 2, ArtistType = ArtistType.Vocaloid });
			_user = Save(new User("Miku", "123", string.Empty, PasswordHashAlgorithms.Default) { Id = 1 });
			_creator = Save(new User("Rin", "123", string.Empty, PasswordHashAlgorithms.Default) { Id = 2 });

			Save(_user.AddArtist(_producer));
		}

		[TestMethod]
		public async Task SendNotifications()
		{
			await CallSendNotifications(_creator);

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(_user, notification.Receiver, "Receiver");
			Assert.AreEqual("New album (original album) by Tripshots", notification.Subject, "Subject");
		}

		[TestMethod]
		public async Task SendNotifications_Email()
		{
			_user.Email = "miku@vocadb.net";
			_user.AllArtists.First().EmailNotifications = true;

			await CallSendNotifications(_creator);

			Assert.IsNotNull(_mailer.Body, "Body");
			Assert.AreEqual(_user.Name, _mailer.ReceiverName, "ReceiverName");
			Assert.AreEqual("New album (original album) by Tripshots", _mailer.Subject, "Subject");
		}

		[TestMethod]
		public async Task SendNotifications_SameUser()
		{
			await CallSendNotifications(_user);

			Assert.IsFalse(_repository.List<UserMessage>().Any(), "No notification created");
		}

		[TestMethod]
		public async Task SendNotifications_DisabledUser()
		{
			_user.Active = false;
			await CallSendNotifications(_creator);

			Assert.IsFalse(_repository.List<UserMessage>().Any(), "No notification created");
		}

		[TestMethod]
		public async Task SendNotifications_MultipleFollowedArtists()
		{
			Save(_user.AddArtist(_vocalist));

			await CallSendNotifications(_creator);

			var notification = _repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual("New album (original album)", notification.Subject, "Subject");
		}

		// User has too many unread notifications
		[TestMethod]
		public async Task TooManyUnreadMessages()
		{
			for (int i = 0; i < 10; ++i)
			{
				_user.ReceivedMessages.Add(_repository.Save(new UserMessage(_user, "New message!", i.ToString(), false)));
			}

			await CallSendNotifications(_creator);

			Assert.AreEqual(10, _repository.List<UserMessage>().Count, "No notification created");
			Assert.IsTrue(_repository.List<UserMessage>().All(m => m.Subject == "New message!"), "No notification created");
		}

		// Too many messages limit only counts notifications
		[TestMethod]
		public async Task OnlyCountNotifications()
		{
			// Not counted since the messages are not notifications
			for (int i = 0; i < 5; ++i)
			{
				_user.ReceivedMessages.Add(_repository.Save(UserMessage.CreateReceived(_creator, _user, "New message!", i.ToString(), false)));
				_user.ReceivedMessages.Add(_repository.Save(UserMessage.CreateSent(_creator, _user, "New message!", i.ToString(), false)));
			}

			Assert.AreEqual(10, _repository.List<UserMessage>().Count, "Number of messages before sending");

			await CallSendNotifications(_creator);

			Assert.AreEqual(11, _repository.List<UserMessage>().Count, "Number of messages after sending");

			var notification = _repository.List<UserMessage>().FirstOrDefault(m => m.Subject != "New message!");

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(_user, notification.Receiver, "Receiver");
		}
	}
}
