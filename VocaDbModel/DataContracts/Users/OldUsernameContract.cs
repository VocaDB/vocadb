#nullable disable

using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class OldUsernameContract
	{
		public OldUsernameContract() { }

#nullable enable
		public OldUsernameContract(OldUsername oldUsername)
		{
			ParamIs.NotNull(() => oldUsername);

			Date = oldUsername.Date;
			OldName = oldUsername.OldName;
		}
#nullable disable

		public DateTime Date { get; init; }

		public string OldName { get; init; }
	}
}
