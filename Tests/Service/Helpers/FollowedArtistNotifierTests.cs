using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Service.Helpers {

	/// <summary>
	/// Tests for <see cref="FollowedArtistNotifier"/>
	/// </summary>
	[TestClass]
	public class FollowedArtistNotifierTests {

		private Album album;
		private User creator;
		private FakeEntryLinkFactory entryLinkFactory;
		private FakeUserMessageMailer mailer;
		private Artist producer;
		private FakeRepository<UserMessage> repository;  
		private User user;
		private Artist vocalist;

		private void CallSendNotifications(IUser creator) {

			repository.HandleTransaction(ctx => {
				new FollowedArtistNotifier().SendNotifications(ctx, album, new[] { producer, vocalist }, creator, entryLinkFactory, mailer);
			});

		}

		private T Save<T>(T entry) {
			return repository.Save(entry);
		}

		[TestInitialize]
		public void SetUp() {

			entryLinkFactory = new FakeEntryLinkFactory();
			repository = new FakeRepository<UserMessage>();
			mailer = new FakeUserMessageMailer();

			album = Save(new Album(new LocalizedString("New Album", ContentLanguageSelection.English)));
			producer = Save(new Artist(TranslatedString.Create("Tripshots")) { Id = 1, ArtistType = ArtistType.Producer });
			vocalist = Save(new Artist(TranslatedString.Create("Hatsune Miku")) { Id = 2, ArtistType = ArtistType.Vocaloid });
			user = Save(new User("Miku", "123", string.Empty, 0) { Id = 1});
			creator = Save(new User("Rin", "123", string.Empty, 0) { Id = 2 });

			Save(user.AddArtist(producer));

		}

		[TestMethod]
		public void SendNotifications() {
			

			CallSendNotifications(creator);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual(user, notification.Receiver, "Receiver");
			Assert.AreEqual("New album by Tripshots", notification.Subject, "Subject");

		}

		[TestMethod]
		public void SendNotifications_Email() {
			
			user.Email = "miku@vocadb.net";
			user.AllArtists.First().EmailNotifications = true;

			CallSendNotifications(creator);

			Assert.IsNotNull(mailer.Body, "Body");
			Assert.AreEqual(user.Name, mailer.ReceiverName, "ReceiverName");
			Assert.AreEqual("New album by Tripshots", mailer.Subject, "Subject");

		}

		[TestMethod]
		public void SendNotifications_SameUser() {

			CallSendNotifications(user);

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification created");

		}

		[TestMethod]
		public void SendNotifications_DisabledUser() {

			user.Active = false;
			CallSendNotifications(creator);

			Assert.IsFalse(repository.List<UserMessage>().Any(), "No notification created");

		}

		[TestMethod]
		public void SendNotifications_MultipleFollowedArtists() {

			Save(user.AddArtist(vocalist));

			CallSendNotifications(creator);

			var notification = repository.List<UserMessage>().FirstOrDefault();

			Assert.IsNotNull(notification, "Notification was created");
			Assert.AreEqual("New album", notification.Subject, "Subject");

		}

		[TestMethod]	
		public void TooManyUnreadMessages() {

			for (int i = 0; i < 10; ++i) {
				user.ReceivedMessages.Add(repository.Save(new UserMessage(user, "New message!", i.ToString(), false)));
			}

			CallSendNotifications(creator);

			Assert.AreEqual(10, repository.List<UserMessage>().Count, "No notification created");
			Assert.IsTrue(repository.List<UserMessage>().All(m => m.Subject == "New message!"), "No notification created");

		}

	}

}
