using VocaDb.Model.Domain.Tags;
using System.Runtime.Serialization;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagUsageContract {

		public TagUsageContract() { }

		public TagUsageContract(TagUsage usage) {

			ParamIs.NotNull(() => usage);

			Count = usage.Count;
			Id = usage.Id;
			TagId = usage.Tag.Id;
			TagName = usage.Tag.Name;

		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public long Id { get; set; }

		[DataMember]
		public int TagId { get; set; }

		[DataMember]
		public string TagName { get; set; }

	}
}
