using System.Runtime.Serialization;
using VocaDb.Model.Domain;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class OptionalDateTimeContract : IOptionalDateTime
	{
		public OptionalDateTimeContract() { }

		public OptionalDateTimeContract(OptionalDateTime dateTime)
		{
			ParamIs.NotNull(() => dateTime);

			Day = dateTime.Day;
			IsEmpty = dateTime.IsEmpty;
			Month = dateTime.Month;
			Year = dateTime.Year;
			Formatted = dateTime.ToString();
		}

		[DataMember]
		public int? Day { get; set; }

		[DataMember]
		public string Formatted { get; set; }

		[DataMember]
		public bool IsEmpty { get; set; }

		[DataMember]
		public int? Month { get; set; }

		[DataMember]
		public int? Year { get; set; }
	}
}
