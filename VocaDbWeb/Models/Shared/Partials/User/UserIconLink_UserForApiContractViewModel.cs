using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Helpers;

namespace VocaDb.Web.Models.Shared.Partials.User
{
	public class UserIconLink_UserForApiContractViewModel
	{
		public UserIconLink_UserForApiContractViewModel(UserForApiContract user, int size = ImageHelper.UserTinyThumbSize, bool userInfo = false, bool tooltip = false)
		{
			User = user;
			Size = size;
			UserInfo = userInfo;
			Tooltip = tooltip;
		}

		public UserForApiContract User { get; set; }

		public int Size { get; set; }

		public bool UserInfo { get; set; }

		public bool Tooltip { get; set; }
	}
}