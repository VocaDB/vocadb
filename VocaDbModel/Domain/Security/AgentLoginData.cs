#nullable disable

using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security
{
	public class AgentLoginData
	{
#nullable enable
		public AgentLoginData(string name)
		{
			ParamIs.NotNullOrEmpty(() => name);

			Name = name;
		}

		public AgentLoginData(User user)
		{
			ParamIs.NotNull(() => user);

			Name = user.Name;
			User = user;
		}
#nullable disable

		public AgentLoginData(User user, string name)
		{
			User = user;
			Name = name;
		}

		public string Name { get; private set; }

		public User User { get; private set; }

		public string UserNameOrFallback => User != null ? User.Name : Name;

		public override string ToString()
		{
			return (User != null ? User.ToString() : Name);
		}
	}
}
