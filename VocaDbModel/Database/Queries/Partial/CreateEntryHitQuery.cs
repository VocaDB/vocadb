using System.Data;
using NHibernate;
using NHibernate.Exceptions;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Database.Queries.Partial;

public class CreateEntryHitQuery
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	public void CreateHit<TEntry, THit>(
		IDatabaseContext ctx,
		TEntry entry,
		string hostname,
		IUserPermissionContext userContext,
		Func<TEntry, int, THit> factory
	)
		where TEntry : class, IEntryBase
		where THit : GenericEntryHit<TEntry>
	{
		if (!userContext.IsLoggedIn && string.IsNullOrEmpty(hostname))
			return;

		var agentNum = userContext.IsLoggedIn ? userContext.LoggedUserId : hostname.GetHashCode();

		if (agentNum == 0)
			return;

		using var tx = ctx.BeginTransaction(IsolationLevel.ReadUncommitted);
		var entryId = entry.Id;
		var isHit = ctx.Query<THit>().Any(h => h.Entry.Id == entryId && h.Agent == agentNum);

		if (!isHit)
		{
			var hit = factory(entry, agentNum);
			try
			{
				ctx.Save(hit);
			}
			catch (GenericADOException x)
			{
				// This can happen if the uniqueness constraint is violated. We could use pessimistic locking, but it's not important enough here.
				s_log.Warn("Unable to save hit for {0}: {1}", entry, x.Message);
				return;
			}

			try
			{
				tx.Commit();
			}
			catch (TransactionException x)
			{
				s_log.Warn(x, "Unable to save hit for {0}", entry);
			}
		}
	}
}
