#nullable disable

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
		private static readonly string _lockStr = "lock";
		private static readonly Logger _log = LogManager.GetCurrentClassLogger();

		public IPRuleManager()
		{
			_permBannedIPs = new HostCollection();
		}

		public IPRuleManager(IEnumerable<string> ips)
		{
			_permBannedIPs = new HostCollection(ips);
		}

		private readonly HostCollection _permBannedIPs;
		private readonly HostCollection _tempBannedIPs = new();

		public IHostCollection PermBannedIPs => _permBannedIPs;

		/// <summary>
		/// Temporarily banned IPs. These are persisted in memory and are cleared on application restart.
		/// </summary>
		public IHostCollection TempBannedIPs => _tempBannedIPs;

		public bool AddPermBannedIP(IDatabaseContext db, IPRule ipRule)
		{
			if (ipRule == null)
				throw new ArgumentNullException(nameof(ipRule));

			lock (_lockStr)
			{
				if (_permBannedIPs.Contains(ipRule.Address))
					return false;

				db.Save(ipRule);

				_permBannedIPs.Add(ipRule.Address);
				return true;
			}
		}

		public bool AddPermBannedIP(IDatabaseContext db, string host, string notes)
			=> AddPermBannedIP(db, new IPRule(host, notes));

		public void AddTempBannedIP(string host, string reason = "")
		{
			_log.Info("Adding temp banned IP {0}. Reason: {1}", host, reason);
			_tempBannedIPs.Add(host);
		}

		/// <summary>
		/// Tests whether a host address is allowed.
		/// Both permanent and temporary rules are checked.
		/// </summary>
		/// <param name="hostAddress">Host address (IP).</param>
		/// <returns>True if the host is allowed access (not banned).</returns>
		public bool IsAllowed(string hostAddress)
		{
			return !_permBannedIPs.Contains(hostAddress) && !_tempBannedIPs.Contains(hostAddress);
		}

		public void RemovePermBannedIP(string address)
		{
			_permBannedIPs.Remove(address);
		}

		public void Reset(IEnumerable<string> ips)
		{
			_permBannedIPs.Reset(ips);
		}
	}
}