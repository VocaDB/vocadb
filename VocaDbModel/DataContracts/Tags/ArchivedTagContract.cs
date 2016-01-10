using System.Linq;
using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedTagContract {

		public ArchivedTagContract() { }

		public ArchivedTagContract(Tag tag, TagDiff diff) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? new ObjectRefContract(tag.AliasedTo) : null;
			CategoryName = tag.CategoryName;
			Description = tag.Description;
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
