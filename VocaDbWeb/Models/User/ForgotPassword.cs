using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models.User
{

	public class ForgotPassword
	{

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.ForgotPasswordStrings), ErrorMessageResourceName = "EmailIsRequired")]
		[StringLength(50)]
		public string Email { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.ForgotPasswordStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[StringLength(100, MinimumLength = 3)]
		public string Username { get; set; }

	}

}