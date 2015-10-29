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
			Tag = new TagBaseContract(usage.Tag);

		}

		[DataMember]
		public int Count { get; set; }

		[DataMember]
		public long Id { get; set; }

		[DataMember]
		public TagBaseContract Tag { get; set; }

	}

}
