using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Mapping.Users
{
	public class UserWebLinkMap : WebLinkMap<UserWebLink, User>
	{
		public UserWebLinkMap() : base(false) { }
	}
}
