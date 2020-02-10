using System;
using System.Threading.Tasks;

namespace VocaDb.Model.Database.Repositories {

	/// <summary>
	/// Minimal interface for transactions. Allows committing and rolling back the transaction.
	/// </summary>
	/// <remarks>
	/// This interface is similar to <see cref="System.Data.IDbTransaction"/> but doesn't provide access to the underlying connection.
	/// </remarks>
	public interface IMinimalTransaction : IDisposable {

		void Commit();

		Task CommitAsync();

		void Rollback();

	}

}
