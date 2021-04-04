#nullable disable

using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.DataContracts
{
	[DataContract(Namespace = Schemas.VocaDb)]
	public class WebLinkForApiContract : IWebLinkWithDescriptionOrUrl
	{
		public WebLinkForApiContract() { }

#nullable enable
		public WebLinkForApiContract(WebLink webLink, WebLinkOptionalFields fields = WebLinkOptionalFields.None)
		{
			ParamIs.NotNull(() => webLink);

			Category = webLink.Category;
			Description = webLink.Description;

			if (fields.HasFlag(WebLinkOptionalFields.DescriptionOrUrl))
				DescriptionOrUrl = webLink.DescriptionOrUrl;

			Disabled = webLink.Disabled;
			Id = webLink.Id;
			Url = webLink.Url;
		}
#nullable disable

		[DataMember]
		[JsonConverter(typeof(StringEnumConverter))]
		public WebLinkCategory Category { get; set; }

		[DataMember]
		public string Description { get; set; }

		[DataMember(EmitDefaultValue = false)]
		public string DescriptionOrUrl { get; init; }

		[DataMember]
		public bool Disabled { get; set; }

		[DataMember]
		public int Id { get; init; }

		[DataMember]
		public string Url { get; set; }
	}

	[Flags]
	public enum WebLinkOptionalFields
	{
		None = 0,
		DescriptionOrUrl = 1 << 0,
	}
}
