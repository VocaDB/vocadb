#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventContract
	{
		private static void DoIfExists(ArchivedReleaseEventVersion version, ReleaseEventEditableFields field,
			XmlCache<ArchivedEventContract> xmlCache, Action<ArchivedEventContract> func)
		{
			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField?.Data != null)
			{
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}
		}

		public static ArchivedEventContract GetAllProperties(ArchivedReleaseEventVersion version)
		{
			var data = new ArchivedEventContract();
			var xmlCache = new XmlCache<ArchivedEventContract>();
			var thisVersion = version.Data != null ? xmlCache.Deserialize(version.Version, version.Data) : new ArchivedEventContract();

			data.Category = thisVersion.Category;
			data.Date = thisVersion.Date;
			data.Description = thisVersion.Description;
			data.Id = thisVersion.Id;
			data.MainPictureMime = thisVersion.MainPictureMime;
			data.Series = thisVersion.Series;
			data.SeriesNumber = thisVersion.SeriesNumber;
			data.SongList = thisVersion.SongList;
			data.TranslatedName = thisVersion.TranslatedName;
			data.Venue = thisVersion.Venue;
			data.VenueName = thisVersion.VenueName;

			DoIfExists(version, ReleaseEventEditableFields.Artists, xmlCache, v => data.Artists = v.Artists);
			DoIfExists(version, ReleaseEventEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, ReleaseEventEditableFields.PVs, xmlCache, v => data.PVs = v.PVs);
			DoIfExists(version, ReleaseEventEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;
		}

		public ArchivedEventContract() { }

#nullable enable
		public ArchivedEventContract(ReleaseEvent ev, ReleaseEventDiff diff)
		{
			ParamIs.NotNull(() => ev);
			ParamIs.NotNull(() => diff);

			Artists = diff.IncludeArtists ? ev.Artists.Select(l => new ArchivedArtistForEventContract(l)).ToArray() : null;
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
			TranslatedName = new ArchivedTranslatedStringContract(ev.TranslatedName);
			Venue = ObjectRefContract.Create(ev.Venue);
			VenueName = ev.VenueName;
			WebLinks = diff.IncludeWebLinks ? ev.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;
		}
#nullable disable

		[DataMember]
		public ArchivedArtistForEventContract[] Artists { get; set; }

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
		public string Name { get; init; }

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
		public ObjectRefContract Venue { get; set; }

		[DataMember]
		public string VenueName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }
	}
}
