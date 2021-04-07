#nullable disable

using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserIconLinkOrName_UserForApiContractViewModel
	{
		public UserIconLinkOrName_UserForApiContractViewModel(UserForApiContract user, string name, int size = 20)
		{
			User = user;
			Name = name;
			Size = size;
		}

		public UserForApiContract User { get; set; }

		public string Name { get; set; }

		public int Size { get; set; }
	}
}