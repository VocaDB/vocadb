using System;
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

			Action = entry.Action;
			AgentName = entry.AgentName;
			Id = entry.Id;
			Time = entry.Time;
			User = (entry.User != null ? new UserContract(entry.User) : null);
		}

		public string Action { get; set; }

		public string AgentName { get; set; }

		public long Id { get; set; }

		public DateTime Time { get; set; }

		public UserContract User { get; set; }
	}
}
