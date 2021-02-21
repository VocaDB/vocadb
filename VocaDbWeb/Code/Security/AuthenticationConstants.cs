namespace VocaDb.Web.Code.Security
{
	public static class AuthenticationConstants
	{
		/// <summary>
		/// The Twitter information comes back in the “ExternalCookie”.
		/// <seealso href="https://blogs.lessthandot.com/index.php/webdev/serverprogramming/aspnet/adding-twitter-authentication-to-an-asp-net-core-2-site-w-cosmos-db/"/>
		/// </summary>
		public const string ExternalCookie = nameof(ExternalCookie);

		/// <summary>
		/// CORS policy for APIs that require authentication. Origins are restricted.
		/// </summary>
		public const string AuthenticatedCorsApiPolicy = nameof(AuthenticatedCorsApiPolicy);
	}
}
