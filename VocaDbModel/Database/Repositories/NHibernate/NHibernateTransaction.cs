#nullable disable

using NHibernate;
using System.Threading.Tasks;

namespace VocaDb.Model.Database.Repositories.NHibernate
{
	public class NHibernateTransaction : IMinimalTransaction
	{
		private readonly ITransaction _tx;

#nullable enable
		public NHibernateTransaction(ITransaction tx)
		{
			ParamIs.NotNull(() => tx);
			_tx = tx;
		}
#nullable disable

		public void Dispose() => _tx.Dispose();

		public void Commit() => _tx.Commit();

		public Task CommitAsync() => _tx.CommitAsync();

		public void Rollback() => _tx.Rollback();
	}
}
