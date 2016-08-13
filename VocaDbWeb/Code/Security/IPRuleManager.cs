using System.Collections.Generic;
using VocaDb.Model.Service.Security;

namespace VocaDb.Web.Code.Security {

	/// <summary>
	/// Manages IP rules. 
	/// Currently this means IPs banned through the admin or automatically.
	/// </summary>
	public class IPRuleManager {

		private readonly HostCollection permBannedIPs;
		private readonly HostCollection tempBannedIPs = new HostCollection();

		public IPRuleManager(IEnumerable<string> ips) {
			this.permBannedIPs = new HostCollection(ips);
		}

		/// <summary>
		/// Temporarily banned IPs. These are persisted in memory and are cleared on application restart.
		/// </summary>
		public HostCollection TempBannedIPs => tempBannedIPs;

		/// <summary>
		/// Tests whether a host address is allowed.
		/// Both permanent and temporary rules are checked.
		/// </summary>
		/// <param name="hostAddress">Host address (IP).</param>
		/// <returns>True if the host is allowed access (not banned).</returns>
		public bool IsAllowed(string hostAddress) {
			return !permBannedIPs.Contains(hostAddress) && !TempBannedIPs.Contains(hostAddress);
		}

		public void Reset(IEnumerable<string> ips) {
			permBannedIPs.Reset(ips);
		}

	}

}