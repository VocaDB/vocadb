using VocaDb.Model.Database.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeTransaction : IMinimalTransaction {

		public void Dispose() {
			
		}

		public void Commit() {
			
		}

		public void Rollback() {
			
		}

	}

}
