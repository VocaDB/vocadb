using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Tests.Domain.PVs
{
	[TestClass]
	public class PVServiceTests
	{
		[DataRow("NicoNicoDouga", nameof(PVService.NicoNicoDouga))]
		[DataRow("Youtube", nameof(PVService.Youtube))]
		[DataRow("SoundCloud", nameof(PVService.SoundCloud))]
		[DataRow("Vimeo", nameof(PVService.Vimeo))]
		[DataRow("Piapro", nameof(PVService.Piapro))]
		[DataRow("Bilibili", nameof(PVService.Bilibili))]
		[DataRow("File", nameof(PVService.File))]
		[DataRow("LocalFile", nameof(PVService.LocalFile))]
		[DataRow("Creofuga", nameof(PVService.Creofuga))]
		[DataRow("Bandcamp", nameof(PVService.Bandcamp))]
		[TestMethod]
		public void Name(string expected, string actual)
		{
			actual.Should().Be(expected);
		}
	}
}
