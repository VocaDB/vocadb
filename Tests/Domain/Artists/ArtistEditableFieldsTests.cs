using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Tests.Domain.Artists
{
	[TestClass]
	public class ArtistEditableFieldsTests
	{
		[DataRow("Nothing", nameof(ArtistEditableFields.Nothing))]
		[DataRow("Albums", nameof(ArtistEditableFields.Albums))]
		[DataRow("ArtistType", nameof(ArtistEditableFields.ArtistType))]
		[DataRow("BaseVoicebank", nameof(ArtistEditableFields.BaseVoicebank))]
		[DataRow("Description", nameof(ArtistEditableFields.Description))]
		[DataRow("Groups", nameof(ArtistEditableFields.Groups))]
		[DataRow("Names", nameof(ArtistEditableFields.Names))]
		[DataRow("OriginalName", nameof(ArtistEditableFields.OriginalName))]
		[DataRow("Picture", nameof(ArtistEditableFields.Picture))]
		[DataRow("Pictures", nameof(ArtistEditableFields.Pictures))]
		[DataRow("ReleaseDate", nameof(ArtistEditableFields.ReleaseDate))]
		[DataRow("Status", nameof(ArtistEditableFields.Status))]
		[DataRow("WebLinks", nameof(ArtistEditableFields.WebLinks))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			actual.Should().Be(expected);
		}
	}
}
