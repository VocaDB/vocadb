using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserLinkViewModel
	{
		public UserLinkViewModel(IUser user)
		{
			User = user;
		}

		public IUser User { get; set; }
	}
}