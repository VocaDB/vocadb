#nullable disable

using System;

namespace VocaDb.Model.Domain.Users
{
	[Flags]
	public enum UserEmailOptions
	{
		NoEmail = 0,

		PrivateMessagesFromAdmins = 1 << 0,

		PrivateMessagesFromAll = 1 << 1,
	}
}
