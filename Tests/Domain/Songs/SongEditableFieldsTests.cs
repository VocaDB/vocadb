using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.Domain.Songs
{
	[TestClass]
	public class SongEditableFieldsTests
	{
		[DataRow("Nothing", nameof(SongEditableFields.Nothing))]
		[DataRow("Albums", nameof(SongEditableFields.Albums))]
		[DataRow("Artists", nameof(SongEditableFields.Artists))]
		[DataRow("Length", nameof(SongEditableFields.Length))]
		[DataRow("Lyrics", nameof(SongEditableFields.Lyrics))]
		[DataRow("Names", nameof(SongEditableFields.Names))]
		[DataRow("Notes", nameof(SongEditableFields.Notes))]
		[DataRow("OriginalName", nameof(SongEditableFields.OriginalName))]
		[DataRow("OriginalVersion", nameof(SongEditableFields.OriginalVersion))]
		[DataRow("PublishDate", nameof(SongEditableFields.PublishDate))]
		[DataRow("PVs", nameof(SongEditableFields.PVs))]
		[DataRow("ReleaseEvent", nameof(SongEditableFields.ReleaseEvent))]
		[DataRow("SongType", nameof(SongEditableFields.SongType))]
		[DataRow("Status", nameof(SongEditableFields.Status))]
		[DataRow("WebLinks", nameof(SongEditableFields.WebLinks))]
		[DataRow("Bpm", nameof(SongEditableFields.Bpm))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			Assert.AreEqual(expected, actual);
		}
	}
}
