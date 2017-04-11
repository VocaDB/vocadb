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
			data.HideFromSuggestions = thisVersion.HideFromSuggestions;
			data.Id = thisVersion.Id;
			data.Targets = thisVersion.Targets;
			data.TranslatedName = thisVersion.TranslatedName;

			DoIfExists(version, TagEditableFields.Description, xmlCache, v => {
				data.Description = v.Description;
				data.DescriptionEng = v.DescriptionEng;
			});
			DoIfExists(version, TagEditableFields.Names, xmlCache, v => data.Names = v.Names);
			DoIfExists(version, TagEditableFields.Parent, xmlCache, v => data.Parent = v.Parent);
			DoIfExists(version, TagEditableFields.RelatedTags, xmlCache, v => data.RelatedTags = v.RelatedTags);
			DoIfExists(version, TagEditableFields.WebLinks, xmlCache, v => data.WebLinks = v.WebLinks);

			return data;

		}

		public ArchivedTagContract() {
			Targets = TagTargetTypes.All;
		}

		public ArchivedTagContract(Tag tag, TagDiff diff) : this() {

			ParamIs.NotNull(() => tag);

			CategoryName = tag.CategoryName;
			Description = diff.IncludeDescription ? tag.Description.Original : null;
			DescriptionEng = diff.IncludeDescription ? tag.Description.English : null;
			HideFromSuggestions = tag.HideFromSuggestions;
			Id = tag.Id;
			Names = diff.IncludeNames ? tag.Names.Names.Select(n => new LocalizedStringContract(n)).ToArray() : null;
			Parent = ObjectRefContract.Create(tag.Parent);
			RelatedTags = diff.IncludeRelatedTags ? tag.RelatedTags.Select(t => new ObjectRefContract(t.LinkedTag)).ToArray() : null;
			Targets = tag.Targets;
			ThumbMime = tag.Thumb != null ? tag.Thumb.Mime : null;
			TranslatedName = new ArchivedTranslatedStringContract(tag.TranslatedName);
			WebLinks = diff.IncludeWebLinks ? tag.WebLinks.Links.Select(l => new ArchivedWebLinkContract(l)).ToArray() : null;

		}

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DescriptionEng { get; set; }

		[DataMember]
		public bool HideFromSuggestions { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public LocalizedStringContract[] Names { get; set; }

		[DataMember]
		public ObjectRefContract Parent { get; set; }

		[DataMember]
		public ObjectRefContract[] RelatedTags { get; set; }

		[DataMember]
		public TagTargetTypes Targets { get; set; }

		[DataMember]
		public string ThumbMime { get; set; }

		[DataMember]
		public ArchivedTranslatedStringContract TranslatedName { get; set; }

		[DataMember]
		public ArchivedWebLinkContract[] WebLinks { get; set; }

	}

}
