#nullable disable

using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.Database.Repositories;

public interface IEventRepository : IRepository<ReleaseEvent>
{
}
