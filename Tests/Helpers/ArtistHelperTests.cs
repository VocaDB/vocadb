using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers {

	[TestClass]
	public class ArtistHelperTests {

		private IArtistWithSupport circle;
		private IArtistWithSupport producer;
		private IArtistWithSupport producer2;

		private IArtistWithSupport CreateArtist(ArtistType artistType, string name) {

			var p = new Artist { ArtistType = artistType };
			p.Names.Add(new ArtistName(p, new LocalizedString(name, ContentLanguageSelection.English)));
			return p.AddAlbum(new Album());

		}

		[TestInitialize]
		public void SetUp() {

			circle = CreateArtist(ArtistType.Circle, "S.C.X.");

			producer = CreateArtist(ArtistType.Producer, "devilishP");
			producer2 = CreateArtist(ArtistType.Producer, "40mP");

		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProductCannotHaveVoiceProvider() {
			Assert.IsFalse(ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProductCanHaveGroup() {
			Assert.IsTrue(ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.Group, LinkDirection.ManyToOne));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_VocaloidCanHaveVoiceProvider() {
			Assert.IsTrue(ArtistHelper.CanHaveRelatedArtists(ArtistType.Vocaloid, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_OtherVocalistCanHaveVoiceProvider() {
			Assert.IsTrue(ArtistHelper.CanHaveRelatedArtists(ArtistType.OtherVocalist, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProducerCanHaveVoicesProvided() {
			Assert.IsTrue(ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_VocaloidCannotHaveVoicesProvided() {
			Assert.IsFalse(ArtistHelper.CanHaveRelatedArtists(ArtistType.Vocaloid, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany));
		}

		[TestMethod]
		public void CanHaveRelatedArtists_OtherVocalistCanHaveVoicesProvided() {
			Assert.IsTrue(ArtistHelper.CanHaveRelatedArtists(ArtistType.OtherVocalist, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany));
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
		public void GetMainCircle_HasCircle() {

			producer.Artist.AddGroup(circle.Artist, ArtistLinkType.Group);
			producer2.Artist.AddGroup(circle.Artist, ArtistLinkType.Group);

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

			producer.Artist.AddGroup(circle.Artist, ArtistLinkType.Group);

			var result = ArtistHelper.GetMainCircle(new[] { producer, producer2, circle }, false);

			Assert.IsNull(result, "No common circle found");

		}

		[TestMethod]
		public void GetMainCircle_NoCircle() {

			producer.Artist.AddGroup(circle.Artist, ArtistLinkType.Group);
			producer2.Artist.AddGroup(circle.Artist, ArtistLinkType.Group);

			var result = ArtistHelper.GetMainCircle(new[] { producer, producer2 }, false);

			Assert.IsNull(result, "No circle found");

		}

	}

}
