#nullable disable

using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.DatabaseTests;

[TestClass]
public class PVForSongDatabaseTests
{
	private readonly DatabaseTestContext _context = new();
	private TestDatabase Db => TestContainerManager.TestDatabase;

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public void Save_LongName()
	{
		_context.RunTest(ctx =>
		{
			var contract = new PVContract
			{
				Author = "fixture",
				Name = new string('x', 300),
				PVId = "synthetic-long-title",
				Service = PVService.Youtube,
				PVType = PVType.Other,
				ThumbUrl = string.Empty,
			};

			ctx.Save(new PVForSong(Db.Song, contract));
			ctx.Flush();
		});
	}
}
