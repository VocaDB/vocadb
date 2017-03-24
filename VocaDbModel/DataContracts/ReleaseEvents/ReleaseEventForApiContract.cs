using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.Songs;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.ReleaseEvents;

namespace VocaDb.Model.DataContracts.ReleaseEvents {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ReleaseEventForApiContract : IReleaseEvent, IEntryBase {

		bool IDeletableEntry.Deleted => false;
		string IEntryBase.DefaultName => Name;
		EntryType IEntryBase.EntryType => EntryType.ReleaseEvent;

		public ReleaseEventForApiContract() { }

		public ReleaseEventForApiContract(ReleaseEvent rel, ReleaseEventOptionalFields fields, IEntryThumbPersister thumbPersister, bool ssl) {
			
			Date = rel.Date;
			Id = rel.Id;
			Name = rel.Name;
			SeriesNumber = rel.SeriesNumber;
			SeriesSuffix = rel.SeriesSuffix;
			UrlSlug = rel.UrlSlug;
			Venue = rel.Venue;
			Version = rel.Version;

			if (rel.Series != null) {
				SeriesId = rel.Series.Id;
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Description)) {
				Description = rel.Description;
			}

			if (thumbPersister != null && fields.HasFlag(ReleaseEventOptionalFields.MainPicture) && !string.IsNullOrEmpty(rel.PictureMime)) {
				MainPicture = new EntryThumbForApiContract(EntryThumb.Create(rel), thumbPersister, ssl);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.Series) && rel.Series != null) {
				Series = new ReleaseEventSeriesContract(rel.Series);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.SongList) && rel.SongList != null) {
				SongList = new SongListBaseContract(rel.SongList);
			}

			if (fields.HasFlag(ReleaseEventOptionalFields.WebLinks)) {
				WebLinks = rel.WebLinks.Select(w => new WebLinkForApiContract(w)).ToArray();
			}

		}

		[DataMember]
		public DateTime? Date { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public EntryThumbForApiContract MainPicture { get; set; }

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

		[DataMember]
		public SongListBaseContract SongList { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

		[DataMember]
		public string Venue { get; set; }

		[DataMember]
		public int Version { get; set; }

		[DataMember]
		public WebLinkForApiContract[] WebLinks { get; set; }

	}

	[Flags]
	public enum ReleaseEventOptionalFields {

		None = 0,
		Description = 1,
		MainPicture = 2,
		Series = 4,
		SongList = 8,
		WebLinks = 16

	}

}
