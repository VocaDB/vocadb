using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserLinkOrNameViewModel
	{
		public UserLinkOrNameViewModel(IUser user, string name)
		{
			User = user;
			Name = name;
		}

		public IUser User { get; }

		public string Name { get; }
	}
}