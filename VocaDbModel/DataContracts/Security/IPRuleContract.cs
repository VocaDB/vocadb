using System.Runtime.Serialization;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.DataContracts.Security;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed record IPRuleContract
{
	[DataMember]
	public string Address { get; init; }

	[DataMember]
	public DateTime Created { get; init; }

	[DataMember]
	public int Id { get; init; }

	[DataMember]
	public string Notes { get; init; }

	public IPRuleContract(IPRule rule)
	{
		Address = rule.Address;
		Created = rule.Created.ToUniversalTime();
		Id = rule.Id;
		Notes = rule.Notes;
	}
}
