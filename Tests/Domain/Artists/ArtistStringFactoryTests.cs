using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Domain.Artists
{
	[TestClass]
	public class ArtistStringFactoryTests
	{
		private ArtistStringFactory artistStringFactory;
		private IArtistLinkWithRoles animator;
		private IArtistLinkWithRoles circle;
		private IArtistLinkWithRoles[] producers;
		private IArtistLinkWithRoles[] vocalists;
		private ArtistForAlbum producer;
		private IArtistLinkWithRoles producer2;
		private IArtistLinkWithRoles producer3;
		private IArtistLinkWithRoles producer4;
		private IArtistLinkWithRoles vocalist;
		private IArtistLinkWithRoles vocalist2;
		private IArtistLinkWithRoles vocalist3;
		private IArtistLinkWithRoles vocalist4;

		private ArtistForAlbum CreateArtist(ArtistType artistType, string name)
		{
			var p = new Artist { ArtistType = artistType };
			p.Names.Add(new ArtistName(p, new LocalizedString(name, ContentLanguageSelection.English)));
			return p.AddAlbum(new Album());
		}

		private string GetNames(params IArtistLinkWithRoles[] artists)
		{
			return string.Join(", ", artists.Select(a => a.Artist.DefaultName));
		}

		private string GetArtistString(params IArtistLinkWithRoles[] artists)
		{
			return artistStringFactory.GetArtistString(artists, ContentFocus.Music).Default;
		}

		private void TestGetArtistString(int producerCount, int vocalistCount, string expected, string message = "artist string as expected")
		{
			var result = artistStringFactory.GetArtistString(producers.Take(producerCount).Concat(vocalists.Take(vocalistCount)), ContentFocus.Music);

			Assert.AreEqual(expected, result.Default, message);
		}

		[TestInitialize]
		public void SetUp()
		{
			artistStringFactory = new ArtistStringFactory();

			circle = CreateArtist(ArtistType.Circle, "S.C.X.");
			animator = CreateArtist(ArtistType.Animator, "wakamuraP");
			producer = CreateArtist(ArtistType.Producer, "devilishP");
			producer2 = CreateArtist(ArtistType.Producer, "40mP");
			producer3 = CreateArtist(ArtistType.Producer, "Clean Tears");
			producer4 = CreateArtist(ArtistType.Producer, "Tripshots");
			producers = new[] { producer, producer2, producer3, producer4 };

			vocalist = CreateArtist(ArtistType.Vocaloid, "Hatsune Miku");
			vocalist2 = CreateArtist(ArtistType.Vocaloid, "Kagamine Rin");
			vocalist3 = CreateArtist(ArtistType.Vocaloid, "Kagamine Len");
			vocalist4 = CreateArtist(ArtistType.Vocaloid, "Megurine Luka");
			vocalists = new[] { vocalist, vocalist2, vocalist3, vocalist4 };
		}

		[TestMethod]
		public void GetArtistString_Empty()
		{
			TestGetArtistString(0, 0, string.Empty, "result is empty");
		}

		[TestMethod]
		public void GetArtistString_OneProducer()
		{
			TestGetArtistString(1, 0, producer.Artist.DefaultName, "producer's name");
		}

		[TestMethod]
		public void GetArtistString_TwoProducers()
		{
			TestGetArtistString(2, 0, GetNames(producer, producer2), "artist string has both producers");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVocalist()
		{
			TestGetArtistString(1, 1, string.Format("{0} feat. {1}", producer.Artist.DefaultName, vocalist.Artist.DefaultName), "artist string has producer and vocalist name");
		}

		[TestMethod]
		public void GetArtistString_MultipleProducersAndTwoVocalists()
		{
			// 3 producers and 2 vocalists
			TestGetArtistString(3, 2, string.Format("{0} feat. {1}", GetNames(producer, producer2, producer3), GetNames(vocalist, vocalist2)), "artist string has multiple producers and vocalists");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVariousVocalists()
		{
			TestGetArtistString(1, 4, string.Format("{0} feat. various", producer.Artist.DefaultName), "artist string has producer and various");
		}

		[TestMethod]
		public void GetArtistString_VariousArtists()
		{
			// 4 producers and 2 vocalists, various artists because of >= 4 producers
			TestGetArtistString(4, 2, ArtistHelper.VariousArtists);
		}

		/// <summary>
		/// One producer and circle, producer is shown first.
		/// </summary>
		[TestMethod]
		public void GetArtistString_OneProducerAndCircle_ProducerFirst()
		{
			var result = ArtistHelper.GetArtistString(new[] { circle, producer }, ContentFocus.Music);

			Assert.AreEqual(GetNames(producer, circle), result.Default, "Producer is shown first");
		}

		[TestMethod]
		public void GetArtistString_OnlyVocalist()
		{
			TestGetArtistString(0, 1, vocalist.Artist.DefaultName, "artist string has vocalist name");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_NotVideo()
		{
			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, ContentFocus.Music);

			Assert.AreEqual(producer.Artist.DefaultName, result.Default, "artist string has one producer");
		}

		/// <summary>
		/// One producer and animator, the disc is video. Animator is shown first.
		/// </summary>
		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_IsVideo()
		{
			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, ContentFocus.Video);

			Assert.AreEqual(GetNames(animator, producer), result.Default, "artist string has one producer and animator");
		}

		/// <summary>
		/// The same artist appears as both producer and vocalist - do not duplicate (default VocaDB behavior).
		/// </summary>
		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_DoNotDuplicate()
		{
			producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(producer);

			Assert.AreEqual("devilishP", result, "result");
		}

		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_MultipleArtists_DoNotDuplicate()
		{
			producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(producer, vocalist);

			Assert.AreEqual("devilishP feat. Hatsune Miku", result, "result");
		}

		/// <summary>
		/// The same artist appears as both producer and vocalist - allow duplication (UtaiteDB behavior).
		/// </summary>
		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_AllowDuplicate()
		{
			artistStringFactory = new ArtistStringFactory(true);
			producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(producer);

			Assert.AreEqual("devilishP feat. devilishP", result, "result");
		}

		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_MultipleArtists_AllowDuplicate()
		{
			artistStringFactory = new ArtistStringFactory(true);
			producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(producer, vocalist);

			Assert.AreEqual("devilishP feat. devilishP, Hatsune Miku", result, "result");
		}
	}
}
