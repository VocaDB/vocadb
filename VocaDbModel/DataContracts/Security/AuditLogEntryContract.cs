#nullable disable

using Ganss.XSS;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security;

[Obsolete]
public class AuditLogEntryContract
{
	public AuditLogEntryContract() { }

#nullable enable
	public AuditLogEntryContract(AuditLogEntry entry)
	{
		ParamIs.NotNull(() => entry);

		Action = new HtmlSanitizer().Sanitize(entry.Action);
		AgentName = entry.AgentName;
		Id = entry.Id;
		Time = entry.Time;
		User = (entry.User != null ? new UserForApiContract(entry.User) : null);
	}
#nullable disable

	public string Action { get; init; }

	public string AgentName { get; init; }

	public long Id { get; init; }

	public DateTime Time { get; init; }

	public UserForApiContract User { get; init; }
}
