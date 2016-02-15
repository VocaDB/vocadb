using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class WebLinkForApiContract : IWebLink {

		public WebLinkForApiContract() {}

		public WebLinkForApiContract(WebLink webLink) {
			
			ParamIs.NotNull(() => webLink);

			Category = webLink.Category;
			Description = webLink.Description;
			Id = webLink.Id;
			Url = webLink.Url;

		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public WebLinkCategory Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Url { get; set; }

	}

}
