using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserIconLink_IUserWithEmailViewModel
	{
		public UserIconLink_IUserWithEmailViewModel(IUserWithEmail user, int size = ImageHelper.UserTinyThumbSize)
		{
			User = user;
			Size = size;
		}

		public IUserWithEmail User { get; set; }

		public int Size { get; set; }
	}
}