using System.Runtime.Serialization;
using Ganss.XSS;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record AuditLogEntryForApiContract
{
	[DataMember]
	public string Action { get; init; }
	[DataMember]
	public string AgentName { get; init; }
	[DataMember]
	public long Id { get; init; }
	[DataMember]
	public DateTime Time { get; init; }
	[DataMember]
	public UserForApiContract? User { get; init; }

	public AuditLogEntryForApiContract(AuditLogEntry entry)
	{
		ParamIs.NotNull(() => entry);

		Action = new HtmlSanitizer().Sanitize(entry.Action);
		AgentName = entry.AgentName;
		Id = entry.Id;
		Time = entry.TimeUtc;
		User = entry.User != null ? new UserForApiContract(entry.User) : null;
	}
}
