using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.User
{
	public class RegisterOpenAuthModel
	{
		public RegisterOpenAuthModel() { }

		public RegisterOpenAuthModel(string accessToken, string name, int twitterId, string twitterName)
		{
			AccessToken = accessToken;
			UserName = name;
			TwitterId = twitterId;
			TwitterName = twitterName;
		}

		public string AccessToken { get; set; }

		public int TwitterId { get; set; }

		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Email")]
		[DataType(DataType.EmailAddress)]
		[StringLength(50)]
		public string Email { get; set; }

		public string TwitterName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.CreateStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.CreateStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		[RegularExpression("[a-zA-Z0-9_]+")]
		public string UserName { get; set; }
	}
}