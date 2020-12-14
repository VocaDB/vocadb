#nullable disable

using System.Linq;
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

			Assert.AreEqual(illustrator.Id, result.Illustrator?.Id, "Illustrator");
			Assert.AreEqual(1, result.AssociatedArtists.Length, "Illustrator is included in the associated artists list");
			var managerRole = result.AssociatedArtists.FirstOrDefault();
			Assert.IsNotNull(managerRole, "Manager");
			Assert.AreEqual(illustrator.Id, managerRole.Parent.Id, "Manager is the same as illustrator");
			Assert.AreEqual(ArtistLinkType.Manager, managerRole.LinkType, "LinkType");
		}
	}
}
