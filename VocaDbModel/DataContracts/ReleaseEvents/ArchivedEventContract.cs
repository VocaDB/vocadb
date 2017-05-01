using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventContract {

		public ArchivedEventContract() { }

		public ArchivedEventContract(ReleaseEvent ev, ReleaseEventDiff diff) {

			ParamIs.NotNull(() => ev);
			ParamIs.NotNull(() => diff);

			Category = ev.Category;
			Date = ev.Date;
			Description = ev.Description;
			Id = ev.Id;
			MainPictureMime = ev.PictureMime;
			Names = diff.IncludeNames ? ev.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null;
			PVs = diff.IncludePVs ? ev.PVs.Select(p => new ArchivedPVContract(p)).ToArray() : null;
			Series = ObjectRefContract.Create(ev.Series);
			SeriesNumber = ev.SeriesNumber;
			SongList = ObjectRefContract.Create(ev.SongList);
			VenueName = ev.Venue;
			WebLinks = diff.IncludeWebLinks ? ev.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;

		}

		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string MainPictureMime { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ArchivedPVContract[] PVs { get; set; }

		[DataMember]
		public ObjectRefContract Series { get; set; }

		[DataMember]
		public int SeriesNumber { get; set; }

		[DataMember]
		public ObjectRefContract SongList { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public string VenueName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
