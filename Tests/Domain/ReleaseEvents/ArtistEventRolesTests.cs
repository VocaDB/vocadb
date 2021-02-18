using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Tests.Domain.ReleaseEvents
{
	[TestClass]
	public class ArtistEventRolesTests
	{
		[DataRow(0, ArtistEventRoles.Default)]
		[DataRow(1, ArtistEventRoles.Dancer)]
		[DataRow(2, ArtistEventRoles.DJ)]
		[DataRow(4, ArtistEventRoles.Instrumentalist)]
		[DataRow(8, ArtistEventRoles.Organizer)]
		[DataRow(16, ArtistEventRoles.Promoter)]
		[DataRow(32, ArtistEventRoles.VJ)]
		[DataRow(64, ArtistEventRoles.Vocalist)]
		[DataRow(128, ArtistEventRoles.VoiceManipulator)]
		[DataRow(256, ArtistEventRoles.OtherPerformer)]
		[DataRow(512, ArtistEventRoles.Other)]
		[TestMethod]
		public void Value(int expected, ArtistEventRoles actual)
		{
			Assert.AreEqual(expected, (int)actual);
		}
	}
}
