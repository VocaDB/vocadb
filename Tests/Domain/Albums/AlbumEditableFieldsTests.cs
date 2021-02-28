using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Tests.Domain.Albums
{
	[TestClass]
	public class AlbumEditableFieldsTests
	{
		[DataRow("Nothing", nameof(AlbumEditableFields.Nothing))]
		[DataRow("Artists", nameof(AlbumEditableFields.Artists))]
		[DataRow("Barcode", nameof(AlbumEditableFields.Barcode))]
		[DataRow("Cover", nameof(AlbumEditableFields.Cover))]
		[DataRow("Description", nameof(AlbumEditableFields.Description))]
		[DataRow("Discs", nameof(AlbumEditableFields.Discs))]
		[DataRow("DiscType", nameof(AlbumEditableFields.DiscType))]
		[DataRow("Identifiers", nameof(AlbumEditableFields.Identifiers))]
		[DataRow("Names", nameof(AlbumEditableFields.Names))]
		[DataRow("OriginalName", nameof(AlbumEditableFields.OriginalName))]
		[DataRow("OriginalRelease", nameof(AlbumEditableFields.OriginalRelease))]
		[DataRow("Pictures", nameof(AlbumEditableFields.Pictures))]
		[DataRow("PVs", nameof(AlbumEditableFields.PVs))]
		[DataRow("Status", nameof(AlbumEditableFields.Status))]
		[DataRow("Tracks", nameof(AlbumEditableFields.Tracks))]
		[DataRow("WebLinks", nameof(AlbumEditableFields.WebLinks))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			actual.Should().Be(expected);
		}
	}
}
