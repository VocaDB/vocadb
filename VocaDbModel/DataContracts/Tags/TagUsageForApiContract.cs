using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageForApiContract {

		public TagUsageForApiContract() { }

		public TagUsageForApiContract(TagUsage tagUsage) {
			Name = tagUsage.Tag.Name;
			Count = tagUsage.Count;
		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public string Name { get; set; }

	}
}
