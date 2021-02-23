#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{
	[TestClass]
	public class ArtistHelperTests
	{
		private IArtistLinkWithRoles _circle;
		private IArtistLinkWithRoles _producer;
		private IArtistLinkWithRoles _producer2;

		private IArtistLinkWithRoles CreateArtist(ArtistType artistType, string name)
		{
			var p = new Artist { ArtistType = artistType };
			p.Names.Add(new ArtistName(p, new LocalizedString(name, ContentLanguageSelection.English)));
			return p.AddAlbum(new Album());
		}

		[TestInitialize]
		public void SetUp()
		{
			_circle = CreateArtist(ArtistType.Circle, "S.C.X.");

			_producer = CreateArtist(ArtistType.Producer, "devilishP");
			_producer2 = CreateArtist(ArtistType.Producer, "40mP");
		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProductCannotHaveVoiceProvider()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne).Should().BeFalse();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProductCanHaveGroup()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.Group, LinkDirection.ManyToOne).Should().BeTrue();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_VocaloidCanHaveVoiceProvider()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.Vocaloid, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne).Should().BeTrue();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_OtherVocalistCanHaveVoiceProvider()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.OtherVocalist, ArtistLinkType.VoiceProvider, LinkDirection.ManyToOne).Should().BeTrue();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_ProducerCanHaveVoicesProvided()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.Producer, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany).Should().BeTrue();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_VocaloidCannotHaveVoicesProvided()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.Vocaloid, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany).Should().BeFalse();
		}

		[TestMethod]
		public void CanHaveRelatedArtists_OtherVocalistCanHaveVoicesProvided()
		{
			ArtistHelper.CanHaveRelatedArtists(ArtistType.OtherVocalist, ArtistLinkType.VoiceProvider, LinkDirection.OneToMany).Should().BeTrue();
		}

		[TestMethod]
		public void GetCanonizedName_NotPName()
		{
			var result = ArtistHelper.GetCanonizedName("devilish5150");

			result.Should().Be("devilish5150", "result");
		}

		[TestMethod]
		public void GetCanonizedName_PName()
		{
			var result = ArtistHelper.GetCanonizedName("devilishP");

			result.Should().Be("devilish", "result");
		}

		[TestMethod]
		public void GetCanonizedName_PDashName()
		{
			var result = ArtistHelper.GetCanonizedName("devilish-P");

			result.Should().Be("devilish", "result");
		}

		[TestMethod]
		public void GetMainCircle_HasCircle()
		{
			_producer.Artist.AddGroup(_circle.Artist, ArtistLinkType.Group);
			_producer2.Artist.AddGroup(_circle.Artist, ArtistLinkType.Group);

			var result = ArtistHelper.GetMainCircle(new[] { _producer, _producer2, _circle }, ContentFocus.Music);

			result.Should().Be(_circle.Artist, "Circle was returned");
		}

		[TestMethod]
		public void GetMainCircle_OnlyCircle()
		{
			var result = ArtistHelper.GetMainCircle(new[] { _circle }, ContentFocus.Music);

			result.Should().Be(_circle.Artist, "Circle was returned");
		}

		[TestMethod]
		public void GetMainCircle_ProducerNotInCircle()
		{
			_producer.Artist.AddGroup(_circle.Artist, ArtistLinkType.Group);

			var result = ArtistHelper.GetMainCircle(new[] { _producer, _producer2, _circle }, ContentFocus.Music);

			result.Should().BeNull("No common circle found");
		}

		[TestMethod]
		public void GetMainCircle_NoCircle()
		{
			_producer.Artist.AddGroup(_circle.Artist, ArtistLinkType.Group);
			_producer2.Artist.AddGroup(_circle.Artist, ArtistLinkType.Group);

			var result = ArtistHelper.GetMainCircle(new[] { _producer, _producer2 }, ContentFocus.Music);

			result.Should().BeNull("No circle found");
		}
	}
}
