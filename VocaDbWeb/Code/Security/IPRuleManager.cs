using System.Collections.Generic;
using System.Threading;

namespace VocaDb.Web.Code.Security {

	public class IPRuleManager {

		private HashSet<string> ips;
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();

		public IPRuleManager(IEnumerable<string> ips) {
			this.ips = new HashSet<string>(ips);
		}

		public bool IsAllowed(string hostAddress) {

			readerWriterLock.EnterReadLock();
			try {
				return !ips.Contains(hostAddress);
			} finally {
				readerWriterLock.ExitReadLock();
			}

		}

		public void Reset(IEnumerable<string> ips) {

			readerWriterLock.EnterWriteLock();
			try {
				this.ips = new HashSet<string>(ips);
			} finally {
				readerWriterLock.ExitWriteLock();
			}

		}

	}

}