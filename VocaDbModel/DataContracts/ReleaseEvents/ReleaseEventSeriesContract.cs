using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventSeriesContract : IEntryImageInformation, IEntryWithIntId {

		EntryType IEntryImageInformation.EntryType => EntryType.ReleaseEventSeries;

		string IEntryImageInformation.Mime => PictureMime;

		public ReleaseEventSeriesContract() {
			Description = string.Empty;
		}

		public ReleaseEventSeriesContract(ReleaseEventSeries series)
			: this() {

			ParamIs.NotNull(() => series);

			Category = series.Category;
			Deleted = series.Deleted;
			Description = series.Description;
			Id = series.Id;
			Name = series.Name;
			PictureMime = series.PictureMime;
			Version = series.Version;

		}

		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember]
		public bool Deleted { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string PictureMime { get; set; }

		[DataMember]
		public int Version { get; set; }

		public override string ToString() {
			return string.Format("release event series {0} [{1}]", Name, Id);
		}

	}

}
