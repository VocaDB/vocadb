#nullable disable

using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace VocaDb.Web.Models.User
{
	public class ForgotPassword
	{
		[Required(ErrorMessageResourceType = typeof(ViewRes.User.ForgotPasswordStrings), ErrorMessageResourceName = "EmailIsRequired")]
		[StringLength(50)]
		public string Email { get; set; }

		[JsonProperty("g-recaptcha-response")]
		public string ReCAPTCHAResponse { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.ForgotPasswordStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[StringLength(100, MinimumLength = 3)]
		public string Username { get; set; }
	}
}