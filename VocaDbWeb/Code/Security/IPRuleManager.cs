using System.Collections.Generic;
using System.Threading;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Manages IP rules. 
	/// Currently this means IPs banned through the admin or automatically.
	/// </summary>
	public class IPRuleManager {

		private HashSet<string> ips;
		private readonly ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim();
		private readonly HostCollection tempBannedIPs = new HostCollection();

		public IPRuleManager(IEnumerable<string> ips) {
			this.ips = new HashSet<string>(ips);
		}

		/// <summary>
		/// Temporarily banned IPs. These are persisted in memory and are cleared on application restart.
		/// </summary>
		public HostCollection TempBannedIPs => tempBannedIPs;

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