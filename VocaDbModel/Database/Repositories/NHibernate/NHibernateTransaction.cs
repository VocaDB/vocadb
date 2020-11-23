using NHibernate;
using System.Threading.Tasks;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	public class NHibernateTransaction : IMinimalTransaction
	{

		private readonly ITransaction tx;

		public NHibernateTransaction(ITransaction tx)
		{
			ParamIs.NotNull(() => tx);
			this.tx = tx;
		}

		public void Dispose() => tx.Dispose();

		public void Commit() => tx.Commit();

		public Task CommitAsync() => tx.CommitAsync();

		public void Rollback() => tx.Rollback();

	}
}
