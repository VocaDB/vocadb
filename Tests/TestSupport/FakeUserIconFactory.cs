using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.TestSupport {

	public class FakeUserIconFactory : IUserIconFactory {

		public string GetIconUrl(User user) {
			return string.Empty;
		}

	}
}
