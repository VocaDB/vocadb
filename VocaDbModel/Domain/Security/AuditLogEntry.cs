#nullable disable

using System;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Security
{
	public class AuditLogEntry : IEntryWithLongId
	{
		public const int MaxActionLength = 400;

		private string _action;
		private string _agentName;

		public AuditLogEntry()
		{
			Category = AuditLogCategory.Unspecified;
			Time = DateTime.Now;
		}

#nullable enable
		public AuditLogEntry(AgentLoginData agentLoginData, string action, AuditLogCategory category, GlobalEntryId entryId)
			: this()
		{
			ParamIs.NotNull(() => agentLoginData);
			ParamIs.NotNullOrEmpty(() => action);

			Action = action.Truncate(MaxActionLength);
			AgentName = agentLoginData.Name;
			Category = category;
			User = agentLoginData.User;
			EntryId = entryId;
		}
#nullable disable

		public virtual string Action
		{
			get => _action;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_action = value;
			}
		}

		public virtual string AgentName
		{
			get => _agentName;
			set
			{
				ParamIs.NotNullOrEmpty(() => value);
				_agentName = value;
			}
		}

		public virtual AuditLogCategory Category { get; set; }

		public virtual GlobalEntryId EntryId { get; set; }

		public virtual long Id { get; set; }

		public virtual DateTime Time { get; set; }

		public virtual User User { get; set; }
	}
}
