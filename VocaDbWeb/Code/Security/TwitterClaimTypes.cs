using Microsoft.AspNetCore.Authentication.Twitter;

namespace VocaDb.Web.Code.Security
{
	public static class TwitterClaimTypes
	{
		public const string AccessToken = nameof(TwitterCreatingTicketContext.AccessToken);
	}
}
