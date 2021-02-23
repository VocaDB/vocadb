#nullable disable

using System.Linq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DataContracts.UseCases
{
	[TestClass]
	public class ArtistForEditContractTests
	{
		[TestMethod]
		public void Ctor_IllustratorIsAlsoManager()
		{
			var illustrator = CreateEntry.Artist(ArtistType.Illustrator, id: 1);
			var artist = CreateEntry.Vocalist(id: 2);
			artist.AddGroup(illustrator, ArtistLinkType.Illustrator);
			artist.AddGroup(illustrator, ArtistLinkType.Manager);

			var result = new ArtistForEditContract(artist, ContentLanguagePreference.Default, new InMemoryImagePersister());

			result.Illustrator?.Id.Should().Be(illustrator.Id, "Illustrator");
			result.AssociatedArtists.Length.Should().Be(1, "Illustrator is included in the associated artists list");
			var managerRole = result.AssociatedArtists.FirstOrDefault();
			managerRole.Should().NotBeNull("Manager");
			managerRole.Parent.Id.Should().Be(illustrator.Id, "Manager is the same as illustrator");
			managerRole.LinkType.Should().Be(ArtistLinkType.Manager, "LinkType");
		}
	}
}
