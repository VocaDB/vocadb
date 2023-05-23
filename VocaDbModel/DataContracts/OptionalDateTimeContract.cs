using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts;

[DataContract(Namespace = Schemas.VocaDb)]
public sealed class OptionalDateTimeContract : IOptionalDateTime
{
	[DataMember]
	public int? Day { get; init; }

	[DataMember]
	public bool IsEmpty { get; init; }

	[DataMember]
	public int? Month { get; init; }

	[DataMember]
	public int? Year { get; init; }

#nullable disable
	public OptionalDateTimeContract() { }
#nullable enable

	public OptionalDateTimeContract(OptionalDateTime dateTime)
	{
		ParamIs.NotNull(() => dateTime);

		Day = dateTime.Day;
		IsEmpty = dateTime.IsEmpty;
		Month = dateTime.Month;
		Year = dateTime.Year;
	}
}
