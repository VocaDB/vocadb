using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.ExtLinks;

namespace VocaDb.Model.Domain.Users
{
	public class UserWebLink : GenericWebLink<User>
	{
		public UserWebLink() { }

		public UserWebLink(User user, WebLinkContract contract)
			: base(user, contract) { }
	}
}
