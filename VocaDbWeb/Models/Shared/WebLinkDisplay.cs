using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Web.Models.Shared
{
	public class WebLinkDisplay
	{
		public WebLinkDisplay()
		{
			Category = WebLinkCategory.Other;
			Description = string.Empty;
			Url = string.Empty;
		}

		public WebLinkDisplay(WebLinkContract contract)
		{
			ParamIs.NotNull(() => contract);

			Category = contract.Category;
			Description = contract.Description;
			Id = contract.Id;
			Url = contract.Url;
		}

		[JsonConverter(typeof(StringEnumConverter))]
		public WebLinkCategory Category { get; set; }

		[StringLength(512)]
		public string Description { get; set; }

		public int Id { get; set; }

		[StringLength(512)]
		[DataType(DataType.Url)]
		public string Url { get; set; }

		public WebLinkContract ToContract()
		{
			return new WebLinkContract
			{
				Id = this.Id,
				Description = this.Description ?? string.Empty,
				Url = this.Url,
				Category = this.Category
			};
		}
	}
}