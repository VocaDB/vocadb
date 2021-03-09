#nullable disable

using System;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class OldUsernameContract
	{
		public OldUsernameContract() { }

		public OldUsernameContract(OldUsername oldUsername)
		{
			ParamIs.NotNull(() => oldUsername);

			Date = oldUsername.Date;
			OldName = oldUsername.OldName;
		}

		public DateTime Date { get; init; }

		public string OldName { get; init; }
	}
}
