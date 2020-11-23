using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;

namespace VocaDb.Model.Database.Queries.Partial
{
	public class CreateMergeEntryQuery
	{
		public void CreateMergeEntry<TEntry, TRecord>(IDatabaseContext ctx, int sourceId, TEntry target)
			where TEntry : class, IEntryBase
			where TRecord : MergeRecord<TEntry>, new()
		{
			// Create merge record
			var oldMergeRecord = ctx.Query<TRecord>().FirstOrDefault(m => m.Target.Id == sourceId);

			if (oldMergeRecord != null)
			{
				if (oldMergeRecord.Source == target.Id)
				{
					// Source is same as target, delete record
					ctx.Delete(oldMergeRecord);
				}
				else
				{
					oldMergeRecord.Target = target;
					ctx.Update(oldMergeRecord);
				}
			}
		}
	}
}
