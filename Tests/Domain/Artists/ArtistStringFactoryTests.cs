#nullable disable

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
		private ArtistStringFactory _artistStringFactory;
		private IArtistLinkWithRoles _animator;
		private IArtistLinkWithRoles _circle;
		private IArtistLinkWithRoles[] _producers;
		private IArtistLinkWithRoles[] _vocalists;
		private ArtistForAlbum _producer;
		private IArtistLinkWithRoles _producer2;
		private IArtistLinkWithRoles _producer3;
		private IArtistLinkWithRoles _producer4;
		private IArtistLinkWithRoles _vocalist;
		private IArtistLinkWithRoles _vocalist2;
		private IArtistLinkWithRoles _vocalist3;
		private IArtistLinkWithRoles _vocalist4;

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
			return _artistStringFactory.GetArtistString(artists, ContentFocus.Music).Default;
		}

		private void TestGetArtistString(int producerCount, int vocalistCount, string expected, string message = "artist string as expected")
		{
			var result = _artistStringFactory.GetArtistString(_producers.Take(producerCount).Concat(_vocalists.Take(vocalistCount)), ContentFocus.Music);

			Assert.AreEqual(expected, result.Default, message);
		}

		[TestInitialize]
		public void SetUp()
		{
			_artistStringFactory = new ArtistStringFactory();

			_circle = CreateArtist(ArtistType.Circle, "S.C.X.");
			_animator = CreateArtist(ArtistType.Animator, "wakamuraP");
			_producer = CreateArtist(ArtistType.Producer, "devilishP");
			_producer2 = CreateArtist(ArtistType.Producer, "40mP");
			_producer3 = CreateArtist(ArtistType.Producer, "Clean Tears");
			_producer4 = CreateArtist(ArtistType.Producer, "Tripshots");
			_producers = new[] { _producer, _producer2, _producer3, _producer4 };

			_vocalist = CreateArtist(ArtistType.Vocaloid, "Hatsune Miku");
			_vocalist2 = CreateArtist(ArtistType.Vocaloid, "Kagamine Rin");
			_vocalist3 = CreateArtist(ArtistType.Vocaloid, "Kagamine Len");
			_vocalist4 = CreateArtist(ArtistType.Vocaloid, "Megurine Luka");
			_vocalists = new[] { _vocalist, _vocalist2, _vocalist3, _vocalist4 };
		}

		[TestMethod]
		public void GetArtistString_Empty()
		{
			TestGetArtistString(0, 0, string.Empty, "result is empty");
		}

		[TestMethod]
		public void GetArtistString_OneProducer()
		{
			TestGetArtistString(1, 0, _producer.Artist.DefaultName, "producer's name");
		}

		[TestMethod]
		public void GetArtistString_TwoProducers()
		{
			TestGetArtistString(2, 0, GetNames(_producer, _producer2), "artist string has both producers");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVocalist()
		{
			TestGetArtistString(1, 1, $"{_producer.Artist.DefaultName} feat. {_vocalist.Artist.DefaultName}", "artist string has producer and vocalist name");
		}

		[TestMethod]
		public void GetArtistString_MultipleProducersAndTwoVocalists()
		{
			// 3 producers and 2 vocalists
			TestGetArtistString(3, 2, $"{GetNames(_producer, _producer2, _producer3)} feat. {GetNames(_vocalist, _vocalist2)}", "artist string has multiple producers and vocalists");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVariousVocalists()
		{
			TestGetArtistString(1, 4, $"{_producer.Artist.DefaultName} feat. various", "artist string has producer and various");
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
			var result = ArtistHelper.GetArtistString(new[] { _circle, _producer }, ContentFocus.Music);

			Assert.AreEqual(GetNames(_producer, _circle), result.Default, "Producer is shown first");
		}

		[TestMethod]
		public void GetArtistString_OnlyVocalist()
		{
			TestGetArtistString(0, 1, _vocalist.Artist.DefaultName, "artist string has vocalist name");
		}

		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_NotVideo()
		{
			var result = ArtistHelper.GetArtistString(new[] { _producer, _animator }, ContentFocus.Music);

			Assert.AreEqual(_producer.Artist.DefaultName, result.Default, "artist string has one producer");
		}

		/// <summary>
		/// One producer and animator, the disc is video. Animator is shown first.
		/// </summary>
		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_IsVideo()
		{
			var result = ArtistHelper.GetArtistString(new[] { _producer, _animator }, ContentFocus.Video);

			Assert.AreEqual(GetNames(_animator, _producer), result.Default, "artist string has one producer and animator");
		}

		/// <summary>
		/// The same artist appears as both producer and vocalist - do not duplicate (default VocaDB behavior).
		/// </summary>
		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_DoNotDuplicate()
		{
			_producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(_producer);

			Assert.AreEqual("devilishP", result, "result");
		}

		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_MultipleArtists_DoNotDuplicate()
		{
			_producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(_producer, _vocalist);

			Assert.AreEqual("devilishP feat. Hatsune Miku", result, "result");
		}

		/// <summary>
		/// The same artist appears as both producer and vocalist - allow duplication (UtaiteDB behavior).
		/// </summary>
		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_AllowDuplicate()
		{
			_artistStringFactory = new ArtistStringFactory(true);
			_producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(_producer);

			Assert.AreEqual("devilishP feat. devilishP", result, "result");
		}

		[TestMethod]
		public void ArtistAsBothProducerAndVocalist_MultipleArtists_AllowDuplicate()
		{
			_artistStringFactory = new ArtistStringFactory(true);
			_producer.Roles = ArtistRoles.Composer | ArtistRoles.Vocalist;

			var result = GetArtistString(_producer, _vocalist);

			Assert.AreEqual("devilishP feat. devilishP, Hatsune Miku", result, "result");
		}
	}
}
