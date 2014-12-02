using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Security {

	public class AuditLogEntry {

		public const int MaxActionLength = 400;

		private string action;
		private string agentName;

		public AuditLogEntry() {
			Category = AuditLogCategory.Unspecified;
			Time = DateTime.Now;
		}

		public AuditLogEntry(AgentLoginData agentLoginData, string action, AuditLogCategory category)
			: this() {
			
			ParamIs.NotNull(() => agentLoginData);
			ParamIs.NotNullOrEmpty(() => action);

			Action = action.Truncate(MaxActionLength);
			AgentName = agentLoginData.Name;
			Category = category;
			User = agentLoginData.User;

		}

		public virtual string Action {
			get { return action; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				action = value; 
			}
		}

		public virtual string AgentName {
			get { return agentName; }
			set {
				ParamIs.NotNullOrEmpty(() => value);
				agentName = value; 
			}
		}

		public virtual AuditLogCategory Category { get; set; }

		public virtual long Id { get; set; }

		public virtual DateTime Time { get; set; }

		public virtual User User { get; set; }

	}

}
