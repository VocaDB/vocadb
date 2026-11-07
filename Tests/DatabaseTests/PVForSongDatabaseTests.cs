#nullable disable

using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
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
			ctx.Save(new PVForSong(Db.Song, CreateLongNameContract("synthetic-long-title")));
			ctx.Flush();
		});
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public void Save_LongName_ForAlbum()
	{
		_context.RunTest(ctx =>
		{
			ctx.Save(new PVForAlbum(Db.Album, CreateLongNameContract("synthetic-long-title-album")));
			ctx.Flush();
		});
	}

	[TestMethod]
	[TestCategory(TestCategories.Database)]
	public void Save_LongName_ForEvent()
	{
		_context.RunTest(ctx =>
		{
			ctx.Save(new PVForEvent(Db.ReleaseEvent, CreateLongNameContract("synthetic-long-title-event")));
			ctx.Flush();
		});
	}

	private static PVContract CreateLongNameContract(string pvId) => new()
	{
		Author = "fixture",
		Name = new string('x', 300),
		PVId = pvId,
		Service = PVService.Youtube,
		PVType = PVType.Other,
		ThumbUrl = string.Empty,
	};
}
