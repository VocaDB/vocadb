using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class ArchivedTagContract {

		public ArchivedTagContract() { }

		public ArchivedTagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? new ObjectRefContract(tag.AliasedTo.Id, tag.AliasedTo.EnglishName) : null;
			CategoryName = tag.CategoryName;
			Description = tag.Description;
			EnglishName = tag.EnglishName;
			Id = tag.Id;
			Parent = tag.Parent != null ? new ObjectRefContract(tag.Parent.Id, tag.Parent.EnglishName) : null;
			ThumbMime = tag.Thumb != null ? tag.Thumb.Mime : null;

		}

		[DataMember]
		public ObjectRefContract AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string EnglishName { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public ObjectRefContract Parent { get; set; }

		[DataMember]
		public string ThumbMime { get; set; }

	}

}
