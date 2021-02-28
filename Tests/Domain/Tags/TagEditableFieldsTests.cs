using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Tests.Domain.Tags
{
	[TestClass]
	public class TagEditableFieldsTests
	{
		[DataRow("Nothing", nameof(TagEditableFields.Nothing))]
		[DataRow("AliasedTo", nameof(TagEditableFields.AliasedTo))]
		[DataRow("CategoryName", nameof(TagEditableFields.CategoryName))]
		[DataRow("Description", nameof(TagEditableFields.Description))]
		[DataRow("HideFromSuggestions", nameof(TagEditableFields.HideFromSuggestions))]
		[DataRow("Names", nameof(TagEditableFields.Names))]
		[DataRow("OriginalName", nameof(TagEditableFields.OriginalName))]
		[DataRow("Parent", nameof(TagEditableFields.Parent))]
		[DataRow("Picture", nameof(TagEditableFields.Picture))]
		[DataRow("RelatedTags", nameof(TagEditableFields.RelatedTags))]
		[DataRow("Status", nameof(TagEditableFields.Status))]
		[DataRow("Targets", nameof(TagEditableFields.Targets))]
		[DataRow("WebLinks", nameof(TagEditableFields.WebLinks))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			actual.Should().Be(expected);
		}
	}
}
