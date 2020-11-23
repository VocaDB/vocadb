using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Songs;

namespace VocaDb.Tests.TestSupport
{
	public class FakeSongListRepository : FakeRepository<SongList>, ISongListRepository
	{
	}
}
