#nullable disable

using System.Security.Principal;
using VocaDb.Model.DataContracts.Users;

namespace VocaDb.Model.Service.Security
{
	public class VocaDbPrincipal : GenericPrincipal
	{
		private readonly UserWithPermissionsContract user;

		public VocaDbPrincipal(IIdentity identity, UserWithPermissionsContract user)
			: base(identity, new string[] { })
		{
			this.user = user;
		}

		public UserWithPermissionsContract User => user;
	}
}