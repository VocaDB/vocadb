using System;
using System.Collections.Generic;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service.Security
{

	/// <summary>
	/// Manages IP rules. 
	/// Currently this means IPs banned through the admin or automatically.
	/// </summary>
	public class IPRuleManager
	{

		private static readonly string lockStr = "lock";
		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		public IPRuleManager()
		{
			permBannedIPs = new HostCollection();
		}

		public IPRuleManager(IEnumerable<string> ips)
		{
			permBannedIPs = new HostCollection(ips);
		}

		private readonly HostCollection permBannedIPs;
		private readonly HostCollection tempBannedIPs = new HostCollection();

		public IHostCollection PermBannedIPs => permBannedIPs;

		/// <summary>
		/// Temporarily banned IPs. These are persisted in memory and are cleared on application restart.
		/// </summary>
		public IHostCollection TempBannedIPs => tempBannedIPs;

		public bool AddPermBannedIP(IDatabaseContext db, IPRule ipRule)
		{

			if (ipRule == null)
				throw new ArgumentNullException(nameof(ipRule));

			lock (lockStr)
			{
				if (permBannedIPs.Contains(ipRule.Address))
					return false;

				db.Save(ipRule);

				permBannedIPs.Add(ipRule.Address);
				return true;
			}

		}

		public bool AddPermBannedIP(IDatabaseContext db, string host, string notes)
			=> AddPermBannedIP(db, new IPRule(host, notes));

		public void AddTempBannedIP(string host, string reason = "")
		{
			log.Info("Adding temp banned IP {0}. Reason: {1}", host, reason);
			tempBannedIPs.Add(host);
		}

		/// <summary>
		/// Tests whether a host address is allowed.
		/// Both permanent and temporary rules are checked.
		/// </summary>
		/// <param name="hostAddress">Host address (IP).</param>
		/// <returns>True if the host is allowed access (not banned).</returns>
		public bool IsAllowed(string hostAddress)
		{
			return !permBannedIPs.Contains(hostAddress) && !tempBannedIPs.Contains(hostAddress);
		}

		public void RemovePermBannedIP(string address)
		{
			permBannedIPs.Remove(address);
		}

		public void Reset(IEnumerable<string> ips)
		{
			permBannedIPs.Reset(ips);
		}

	}

}