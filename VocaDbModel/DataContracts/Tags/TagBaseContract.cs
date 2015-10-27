using System.Runtime.Serialization;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.DataContracts.Tags {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class TagBaseContract {

		public TagBaseContract() { }

		public TagBaseContract(Tag tag) {
			
			ParamIs.NotNull(() => tag);

			Id = tag.Id;
			Name = Slug = tag.Name;

		}

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string Slug { get; set; }

	}

}
