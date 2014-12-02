using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Albums;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventSeriesContract {

		public ArchivedEventSeriesContract() { }

		public ArchivedEventSeriesContract(ReleaseEventSeries series) {

			ParamIs.NotNull(() => series);

			Aliases = series.Aliases.Select(a => a.Name).ToArray();
			Description = series.Description;
			Id = series.Id;
			Name = series.Name;

		}

		[DataMember]
		public string[] Aliases { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

	}

}
