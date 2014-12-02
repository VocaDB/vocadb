using System;

namespace VocaDb.Model.Domain.Security {

	[Flags]
	public enum RoleTypes {

		Nothing		= 0,

		User		= 1,

		Admin		= 2

	}

}
