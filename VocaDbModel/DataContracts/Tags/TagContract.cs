using System.Runtime.Serialization;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagContract {

		public TagContract() {
			AliasedTo = string.Empty;
		}

		public TagContract(Tag tag) {

			ParamIs.NotNull(() => tag);

			AliasedTo = tag.AliasedTo != null ? tag.AliasedTo.Name : null;
			CategoryName = tag.CategoryName;
			Description = tag.Description;
			Id = tag.Id;
			Name = tag.TagName;
			Parent = (tag.Parent != null ? tag.Parent.Name : null);
			Status = tag.Status;
			Version = tag.Version;

		}

		[DataMember]
		public string AliasedTo { get; set; }

		[DataMember]
		public string CategoryName { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Parent { get; set; }

		[DataMember]
		public EntryStatus Status { get; set; }

		[DataMember]
		public int Version { get; set; }

	}
}
