#nullable disable

using System;
using System.Runtime.Serialization;
using Ganss.XSS;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security
{
	public class ServerOnlyAuditLogEntryContract
	{
		public ServerOnlyAuditLogEntryContract() { }

#nullable enable
		public ServerOnlyAuditLogEntryContract(AuditLogEntry entry)
		{
			ParamIs.NotNull(() => entry);

			Action = new HtmlSanitizer().Sanitize(entry.Action);
			AgentName = entry.AgentName;
			Id = entry.Id;
			Time = entry.Time;
			User = (entry.User != null ? new ServerOnlyUserContract(entry.User) : null);
		}
#nullable disable

		public string Action { get; init; }

		public string AgentName { get; init; }

		public long Id { get; init; }

		public DateTime Time { get; init; }

		public ServerOnlyUserContract User { get; init; }
	}
}
