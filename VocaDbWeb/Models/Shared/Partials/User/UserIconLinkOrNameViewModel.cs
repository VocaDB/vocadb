using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserIconLinkOrNameViewModel
	{
		public UserIconLinkOrNameViewModel(IUserWithEmail user, string name, int size = 20)
		{
			User = user;
			Name = name;
			Size = size;
		}

		public IUserWithEmail User { get; set; }

		public string Name { get; set; }

		public int Size { get; set; }
	}
}