using System.Runtime.Serialization;

namespace VocaDb.SiteMapGenerator.VocaDb.DataContracts {

	[DataContract]
	public class TagBaseContract {

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public string UrlSlug { get; set; }

	}

}
