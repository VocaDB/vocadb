#nullable disable

using System.Security.Principal;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Service.Security
{
	public class VocaDbPrincipal : GenericPrincipal
	{
		private readonly ServerOnlyUserWithPermissionsContract _user;

		public VocaDbPrincipal(IIdentity identity, ServerOnlyUserWithPermissionsContract user)
			: base(identity, new string[] { })
		{
			_user = user;
		}

		public ServerOnlyUserWithPermissionsContract User => _user;
	}
}