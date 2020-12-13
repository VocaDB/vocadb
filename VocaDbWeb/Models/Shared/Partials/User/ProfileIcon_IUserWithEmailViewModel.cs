using VocaDb.Model.Domain.Users;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class ProfileIcon_IUserWithEmailViewModel
	{
		public ProfileIcon_IUserWithEmailViewModel(IUserWithEmail user, int size = 80)
		{
			User = user;
			Size = size;
		}

		public IUserWithEmail User { get; set; }

		public int Size { get; set; }
	}
}