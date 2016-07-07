using System;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventForApiContract {

		public ReleaseEventForApiContract() { }

		public ReleaseEventForApiContract(ReleaseEvent rel, ReleaseEventOptionalFields fields) {
			
			Date = rel.Date;
			Id = rel.Id;
			Name = rel.Name;
			SeriesNumber = rel.SeriesNumber;
			SeriesSuffix = rel.SeriesSuffix;

			if (rel.Series != null) {
				SeriesId = rel.Series.Id;
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Description)) {
				Description = rel.Description;
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Series) && rel.Series != null) {
				Series = new ReleaseEventSeriesContract(rel.Series);
			}

		}

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public ReleaseEventSeriesContract Series { get; set; }

		[DataMember]
		public int? SeriesId { get; set; }

		[DataMember]
		public int SeriesNumber { get; set; }

		[DataMember]
		public string SeriesSuffix { get; set; }

	}

	[Flags]
	public enum ReleaseEventOptionalFields {

		None = 0,
		Description = 1,
		Series = 2,

	}

}
