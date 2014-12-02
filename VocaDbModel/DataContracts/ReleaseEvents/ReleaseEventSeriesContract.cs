using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventSeriesContract {

		public ReleaseEventSeriesContract() {
			Description = string.Empty;
		}

		public ReleaseEventSeriesContract(ReleaseEventSeries series)
			: this() {

			ParamIs.NotNull(() => series);

			Description = series.Description;
			Id = series.Id;
			Name = series.Name;

		}

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		public override string ToString() {
			return string.Format("release event series {0} [{1}]", Name, Id);
		}

	}

}
