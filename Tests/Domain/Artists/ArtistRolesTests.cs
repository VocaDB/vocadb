using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Artists;

namespace VocaDb.Tests.Domain.Artists
{
	[TestClass]
	public class ArtistRolesTests
	{
		[DataRow(0, ArtistRoles.Default)]
		[DataRow(1, ArtistRoles.Animator)]
		[DataRow(2, ArtistRoles.Arranger)]
		[DataRow(4, ArtistRoles.Composer)]
		[DataRow(8, ArtistRoles.Distributor)]
		[DataRow(16, ArtistRoles.Illustrator)]
		[DataRow(32, ArtistRoles.Instrumentalist)]
		[DataRow(64, ArtistRoles.Lyricist)]
		[DataRow(128, ArtistRoles.Mastering)]
		[DataRow(256, ArtistRoles.Publisher)]
		[DataRow(512, ArtistRoles.Vocalist)]
		[DataRow(1024, ArtistRoles.VoiceManipulator)]
		[DataRow(2048, ArtistRoles.Other)]
		[DataRow(4096, ArtistRoles.Mixer)]
		[DataRow(8192, ArtistRoles.Chorus)]
		[DataRow(16384, ArtistRoles.Encoder)]
		[DataRow(32768, ArtistRoles.VocalDataProvider)]
		[TestMethod]
		public void Value(int expected, ArtistRoles actual)
		{
			Assert.AreEqual(expected, (int)actual);
		}
	}
}
