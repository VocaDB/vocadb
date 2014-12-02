using System;
using System.Collections.Generic;

namespace VocaDb.Web.Code.Security {

	public class IPRuleManager {

		private HashSet<string> ips;
		private const string ipsLock = "lock";
		private readonly Func<string[]> loadFunc;

		public IPRuleManager(Func<string[]> loadFunc) {
			this.loadFunc = loadFunc;
		}

		public bool IsAllowed(string hostAddress) {

			if (ips == null) {
				lock (ipsLock) {
					ips = new HashSet<string>(loadFunc());
				}
			}

			return !ips.Contains(hostAddress);

		}

		public void Reset() {
			lock (ipsLock) {
				ips = null;
			}
		}

	}

}