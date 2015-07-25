using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class ArtistHelperTests {

		private IArtistWithSupport animator;
		private IArtistWithSupport circle;
		private IArtistWithSupport producer;
		private IArtistWithSupport producer2;
		private IArtistWithSupport producer3;
		private IArtistWithSupport producer4;
		private IArtistWithSupport vocalist;
		private IArtistWithSupport vocalist2;
		private IArtistWithSupport vocalist3;
		private IArtistWithSupport vocalist4;

		private IArtistWithSupport[] producers;
		private IArtistWithSupport[] vocalists;

		private IArtistWithSupport CreateArtist(ArtistType artistType, string name) {

			var p = new Artist { ArtistType = artistType };
			p.Names.Add(new ArtistName(p, new LocalizedString(name, ContentLanguageSelection.English)));
			return p.AddAlbum(new Album());

		}

		private string GetNames(params IArtistWithSupport[] artists) {
			return string.Join(", ", artists.Select(a => a.Artist.DefaultName));
		}

		private void TestGetArtistString(int producerCount, int vocalistCount, string expected, string message = "artist string as expected") {

			var result = ArtistHelper.GetArtistString(producers.Take(producerCount).Concat(vocalists.Take(vocalistCount)), false);

			Assert.AreEqual(expected, result.Default, message);

		}

		[TestInitialize]
		public void SetUp() {

			animator = CreateArtist(ArtistType.Animator, "wakamuraP");
			circle = CreateArtist(ArtistType.Circle, "S.C.X.");

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
		public void GetCanonizedName_NotPName() {

			var result = ArtistHelper.GetCanonizedName("devilish5150");

			Assert.AreEqual("devilish5150", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PName() {

			var result = ArtistHelper.GetCanonizedName("devilishP");

			Assert.AreEqual("devilish", result, "result");

		}

		[TestMethod]
		public void GetCanonizedName_PDashName() {

			var result = ArtistHelper.GetCanonizedName("devilish-P");

			Assert.AreEqual("devilish", result, "result");

		}

		[TestMethod]
		public void GetArtistString_Empty() {

			TestGetArtistString(0, 0, string.Empty, "result is empty");

		}

		[TestMethod]
		public void GetArtistString_OneProducer() {

			TestGetArtistString(1, 0, producer.Artist.DefaultName, "producer's name");

		}

		[TestMethod]
		public void GetArtistString_TwoProducers() {

			TestGetArtistString(2, 0, GetNames(producer, producer2), "artist string has both producers");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVocalist() {

			TestGetArtistString(1, 1, string.Format("{0} feat. {1}", producer.Artist.DefaultName, vocalist.Artist.DefaultName), "artist string has producer and vocalist name");

		}

		[TestMethod]
		public void GetArtistString_MultipleProducersAndTwoVocalists() {

			// 3 producers and 2 vocalists
			TestGetArtistString(3, 2, string.Format("{0} feat. {1}", GetNames(producer, producer2, producer3), GetNames(vocalist, vocalist2)), "artist string has multiple producers and vocalists");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndVariousVocalists() {

			TestGetArtistString(1, 4, string.Format("{0} feat. various", producer.Artist.DefaultName), "artist string has producer and various");

		}

		[TestMethod]
		public void GetArtistString_VariousArtists() {

			// 4 producers and 2 vocalists, various artists because of >= 4 producers
			TestGetArtistString(4, 2, ArtistHelper.VariousArtists);

		}

		/// <summary>
		/// One producer and circle, producer is shown first.
		/// </summary>
		[TestMethod]
		public void GetArtistString_OneProducerAndCircle_ProducerFirst() {

			var result = ArtistHelper.GetArtistString(new[] {circle, producer}, false);

			Assert.AreEqual(GetNames(producer, circle), result.Default, "Producer is shown first");

		}

		[TestMethod]
		public void GetArtistString_OnlyVocalist() {

			TestGetArtistString(0, 1, vocalist.Artist.DefaultName, "artist string has vocalist name");

		}

		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_NotVideo() {

			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, false);

			Assert.AreEqual(producer.Artist.DefaultName, result.Default, "artist string has one producer");

		}

		/// <summary>
		/// One producer and animator, the disc is video. Animator is shown first.
		/// </summary>
		[TestMethod]
		public void GetArtistString_OneProducerAndAnimator_IsVideo() {

			var result = ArtistHelper.GetArtistString(new[] { producer, animator }, true);

			Assert.AreEqual(GetNames(animator, producer), result.Default, "artist string has one producer and animator");

		}

		[TestMethod]
		public void GetMainCircle_HasCircle() {

			producer.Artist.AddGroup(circle.Artist);
			producer2.Artist.AddGroup(circle.Artist);

			var result = ArtistHelper.GetMainCircle(new[] { producer, producer2, circle }, false);

			Assert.AreEqual(circle.Artist, result, "Circle was returned");

		}

		[TestMethod]
		public void GetMainCircle_OnlyCircle() {

			var result = ArtistHelper.GetMainCircle(new[] { circle }, false);

			Assert.AreEqual(circle.Artist, result, "Circle was returned");

		}

		[TestMethod]
		public void GetMainCircle_ProducerNotInCircle() {

			producer.Artist.AddGroup(circle.Artist);

			var result = ArtistHelper.GetMainCircle(new[] { producer, producer2, circle }, false);

			Assert.IsNull(result, "No common circle found");

		}

		[TestMethod]
		public void GetMainCircle_NoCircle() {

			producer.Artist.AddGroup(circle.Artist);
			producer2.Artist.AddGroup(circle.Artist);

			var result = ArtistHelper.GetMainCircle(new[] { producer, producer2 }, false);

			Assert.IsNull(result, "No circle found");

		}

	}

}
