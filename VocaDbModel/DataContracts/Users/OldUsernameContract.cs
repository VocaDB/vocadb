using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.DataContracts.Users
{
	public class OldUsernameContract
	{
#nullable disable
		public OldUsernameContract() { }
#nullable enable

		public OldUsernameContract(OldUsername oldUsername)
		{
			ParamIs.NotNull(() => oldUsername);

			Date = oldUsername.DateUtc;
			OldName = oldUsername.OldName;
		}

		public DateTime Date { get; init; }

		public string OldName { get; init; }
	}
}
