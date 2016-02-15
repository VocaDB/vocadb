using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.DataContracts {

	[DataContract(Namespace = Schemas.VocaDb)]
	public class WebLinkContract : IWebLink {

		public WebLinkContract() {
			Category = WebLinkCategory.Other;
		}

		public WebLinkContract(string url, string description, WebLinkCategory category) {

			Url = url;
			Description = description;
			Category = category;

			DescriptionOrUrl = !string.IsNullOrEmpty(description) ? description : url;

		}

		public WebLinkContract(WebLink link) {
			
			ParamIs.NotNull(() => link);

			Category = link.Category;
			Description = link.Description;
			DescriptionOrUrl = link.DescriptionOrUrl;
			Id = link.Id;
			Url = link.Url;

		}

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public WebLinkCategory Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember]
		public string DescriptionOrUrl { get; set; }

		[DataMember]
		public int Id { get; set; }

		[DataMember]
		public string Url { get; set; }

	}

}
