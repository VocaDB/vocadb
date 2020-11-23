using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.User
{

	public class Index
	{
		public string Filter { get; set; }
		[JsonConverter(typeof(StringEnumConverter))]
		public UserGroupId? GroupId { get; set; }
	}

}