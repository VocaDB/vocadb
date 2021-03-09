#nullable disable

using System;
using Ganss.XSS;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security
{
	public class AuditLogEntryContract
	{
		public AuditLogEntryContract() { }

		public AuditLogEntryContract(AuditLogEntry entry)
		{
			ParamIs.NotNull(() => entry);

			Action = new HtmlSanitizer().Sanitize(entry.Action);
			AgentName = entry.AgentName;
			Id = entry.Id;
			Time = entry.Time;
			User = (entry.User != null ? new UserContract(entry.User) : null);
		}

		public string Action { get; init; }

		public string AgentName { get; init; }

		public long Id { get; init; }

		public DateTime Time { get; init; }

		public UserContract User { get; init; }
	}
}
