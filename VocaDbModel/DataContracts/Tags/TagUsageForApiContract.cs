using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageForApiContract {

		public TagUsageForApiContract() { }

		public TagUsageForApiContract(TagUsage tagUsage) {
			Name = tagUsage.Tag.Name;
			TagId = tagUsage.Tag.Id;
			Count = tagUsage.Count;
		}

		public TagUsageForApiContract(TagUsageContract tagUsage) {
			Name = tagUsage.TagName;
			TagId = tagUsage.TagId;
			Count = tagUsage.Count;
		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public int TagId { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
