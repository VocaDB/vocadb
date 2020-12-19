#nullable disable

using System.Security.Principal;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Service.Security
{
	public class VocaDbPrincipal : GenericPrincipal
	{
		private readonly UserWithPermissionsContract _user;

		public VocaDbPrincipal(IIdentity identity, UserWithPermissionsContract user)
			: base(identity, new string[] { })
		{
			_user = user;
		}

		public UserWithPermissionsContract User => _user;
	}
}