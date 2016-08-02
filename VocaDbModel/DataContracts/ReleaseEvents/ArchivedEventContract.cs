using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventContract {

		public ArchivedEventContract() { }

		public ArchivedEventContract(ReleaseEvent ev, ReleaseEventDiff diff) {

			ParamIs.NotNull(() => ev);
			ParamIs.NotNull(() => diff);

			Date = ev.Date;
			Description = ev.Description;
			Id = ev.Id;
			Name = ev.Name;
			Series = (ev.Series != null ? new ObjectRefContract(ev.Series) : null);
			SeriesNumber = ev.SeriesNumber;
			WebLinks = diff.IsIncluded(ReleaseEventEditableFields.WebLinks) ? ev.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;

		}

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public ObjectRefContract Series { get; set; }

		[DataMember]
		public int SeriesNumber { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
