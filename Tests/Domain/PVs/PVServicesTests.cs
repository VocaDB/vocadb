using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.PVs;

namespace VocaDb.Tests.Domain.PVs
{
	[TestClass]
	public class PVServicesTests
	{
		[DataRow(0, PVServices.Nothing)]
		[DataRow(1, PVServices.NicoNicoDouga)]
		[DataRow(2, PVServices.Youtube)]
		[DataRow(4, PVServices.SoundCloud)]
		[DataRow(8, PVServices.Vimeo)]
		[DataRow(16, PVServices.Piapro)]
		[DataRow(32, PVServices.Bilibili)]
		[DataRow(64, PVServices.File)]
		[DataRow(128, PVServices.LocalFile)]
		[DataRow(256, PVServices.Creofuga)]
		[DataRow(512, PVServices.Bandcamp)]
		[TestMethod]
		public void Value(int expected, PVServices actual)
		{
			Assert.AreEqual(expected, (int)actual);
		}
	}
}
