using System;
using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Utils;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedTagContract {

		private static void DoIfExists(ArchivedTagVersion version, TagEditableFields field,
			XmlCache<ArchivedTagContract> xmlCache, Action<ArchivedTagContract> func) {

			var versionWithField = version.GetLatestVersionWithField(field);

			if (versionWithField != null && versionWithField.Data != null) {
				var data = xmlCache.Deserialize(versionWithField.Version, versionWithField.Data);
				func(data);
			}

		}

		public static ArchivedTagContract GetAllProperties(ArchivedTagVersion version) {

			var data = new ArchivedTagContract();
			var xmlCache = new XmlCache<ArchivedTagContract>();
			var thisVersion = version.Data != null ? xmlCache.Deserialize(version.Version, version.Data) : new ArchivedTagContract();

			data.CategoryName = thisVersion.CategoryName;
			data.Id = thisVersion.Id;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, TagEditableFields.AliasedTo, xmlCache, v => data.AliasedTo = v.AliasedTo);
			DoIfExists(version, TagEditableFields.Description, xmlCache, v => {
				data.Description = v.Description;
				data.DescriptionEng = v.DescriptionEng;
			});
			DoIfExists(version, TagEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, TagEditableFields.Parent, xmlCache, v => data.Parent = v.Parent);

			return data;

		}

		public ArchivedTagContract() { }

		public ArchivedTagContract(Tag tag, TagDiff diff) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? new ObjectRefContract(tag.AliasedTo) : null;
			CategoryName = tag.CategoryName;
			Description = diff.IncludeDescription ? tag.Description.Original : null;
			DescriptionEng = diff.IncludeDescription ? tag.Description.English : null;
			Id = tag.Id;
			Names = diff.IncludeNames ? tag.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null;
			Parent = tag.Parent != null ? new ObjectRefContract(tag.Parent) : null;
			ThumbMime = tag.Thumb != null ? tag.Thumb.Mime : null;
			TranslatedName = new ArchivedTranslatedStringContract(tag.TranslatedName);

		}

		[DataMember]
		public ObjectRefContract AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DescriptionEng { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ObjectRefContract Parent { get; set; }

		[DataMember]
		public string ThumbMime { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

	}

}
