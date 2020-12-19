#nullable disable

using System;
using System.Threading.Tasks;
using VocaDb.Model.Database.Repositories;

namespace VocaDb.Tests.TestSupport
{
	public sealed class FakeTransaction : IMinimalTransaction
	{
		public FakeTransaction() { }

		public FakeTransaction(Action commitAction = null, Action rollbackAction = null)
		{
			this._commitAction = commitAction;
			this._rollbackAction = rollbackAction;
		}

		private readonly Action _commitAction;
		private readonly Action _rollbackAction;

		public bool Committed { get; private set; }
		public bool Disposed { get; private set; }

		public void Dispose()
		{
			if (Disposed)
				return;
			if (!Committed)
				Rollback();
			Disposed = true;
		}

		public void Commit()
		{
			if (Disposed)
				throw new InvalidOperationException("Cannot commit after dispose");
			_commitAction?.Invoke();
			Committed = true;
		}

		public Task CommitAsync()
		{
			Commit();
			return Task.CompletedTask;
		}

		public void Rollback()
		{
			_rollbackAction?.Invoke();
		}
	}
}
