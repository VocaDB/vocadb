using System;

namespace VocaDb.Model.Service.Repositories {

	/// <summary>
	/// Minimal interface for transactions. Allows committing and rolling back the transaction.
	/// </summary>
	/// <remarks>
	/// This interface is similar to <see cref="System.Data.IDbTransaction"/> but doesn't provide access to the underlying connection.
	/// </remarks>
	public interface IMinimalTransaction : IDisposable {

		void Commit();

		void Rollback();

	}

}
