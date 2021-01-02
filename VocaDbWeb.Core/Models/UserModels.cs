#nullable disable

using System.ComponentModel.DataAnnotations;

namespace VocaDb.Web.Models
{
	public class LoginModel
	{
		public LoginModel() { }

		public LoginModel(string returnUrl, bool returnToMainSite)
		{
			ReturnUrl = returnUrl;
			ReturnToMainSite = returnToMainSite;
		}

		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "KeepMeLoggedIn")]
		public bool KeepLoggedIn { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "UsernameIsRequired")]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Username")]
		[StringLength(100, MinimumLength = 3)]
		public string UserName { get; set; }

		[Required(ErrorMessageResourceType = typeof(ViewRes.User.LoginStrings), ErrorMessageResourceName = "PasswordIsRequired")]
		[DataType(DataType.Password)]
		[Display(ResourceType = typeof(ViewRes.User.LoginStrings), Name = "Password")]
		[StringLength(100)]
		public string Password { get; set; }

		/// <summary>
		/// Whether the user should be returned to the main (HTTP) site.
		/// 
		/// If this is false, the user will be returned to the current site,
		/// which may be either HTTP or HTTPS.
		/// If this is true, the user will always be returned to the main site.
		/// 
		/// This is needed because the HTTP site uses the secure HTTPS site for logging in
		/// and the user should be returned to back to the main HTTP site after login.
		/// </summary>
		public bool ReturnToMainSite { get; set; }

		public string ReturnUrl { get; set; }
	}
}