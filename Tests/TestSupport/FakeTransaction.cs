using System;
using VocaDb.Model.Database.Repositories;

namespace VocaDb.Tests.TestSupport {

	public sealed class FakeTransaction : IMinimalTransaction {

		public FakeTransaction() { }

		public FakeTransaction(Action commitAction = null, Action rollbackAction = null) {
			this.commitAction = commitAction;
			this.rollbackAction = rollbackAction;
		}

		private readonly Action commitAction;
		private readonly Action rollbackAction;
		
		public bool Committed { get; private set; }
		public bool Disposed { get; private set; }

		public void Dispose() {
			if (Disposed)
				return;
			if (!Committed)
				Rollback();
			Disposed = true;
		}

		public void Commit() {
			if (Disposed)
				throw new InvalidOperationException("Cannot commit after dispose");
			commitAction?.Invoke();
			Committed = true;
		}

		public void Rollback() {
			rollbackAction?.Invoke();
		}

	}

}
