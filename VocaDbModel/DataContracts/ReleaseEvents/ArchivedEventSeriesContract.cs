#nullable disable

using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.ReleaseEvents;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.ReleaseEvents
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedEventSeriesContract
	{
		private static void DoIfExists(ArchivedReleaseEventSeriesVersion version, ReleaseEventSeriesEditableFields field,
			XmlCache<ArchivedEventSeriesContract> xmlCache, Action<ArchivedEventSeriesContract> func)
		{
			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField?.Data != null)
			{
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}
		}

		public static ArchivedEventSeriesContract GetAllProperties(ArchivedReleaseEventSeriesVersion version)
		{
			var data = new ArchivedEventSeriesContract();
			var xmlCache = new XmlCache<ArchivedEventSeriesContract>();
			var thisVersion = version.Data != null ? xmlCache.Deserialize(version.Version, version.Data) : new ArchivedEventSeriesContract();

			data.Category = thisVersion.Category;
			data.Description = thisVersion.Description;
			data.Id = thisVersion.Id;
			data.MainPictureMime = thisVersion.MainPictureMime;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, ReleaseEventSeriesEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, ReleaseEventSeriesEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;
		}

		public ArchivedEventSeriesContract() { }

		public ArchivedEventSeriesContract(ReleaseEventSeries series, ReleaseEventSeriesDiff diff)
		{
			ParamIs.NotNull(() => series);

			Category = series.Category;
			Description = series.Description;
			Id = series.Id;
			MainPictureMime = series.PictureMime;
			Names = diff.IncludeNames ? series.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null;
			TranslatedName = new ArchivedTranslatedStringContract(series.TranslatedName);
			WebLinks = diff.IncludeWebLinks ? series.WebLinks.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;
		}

		[DataMember]
		public string[] Aliases { get; set; }

		[DataMember]
		public EventCategory Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string MainPictureMime { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }
	}
}
