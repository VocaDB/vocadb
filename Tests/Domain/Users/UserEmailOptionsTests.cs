using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Tests.Domain.Users
{
	[TestClass]
	public class UserEmailOptionsTests
	{
		[DataRow(0, UserEmailOptions.NoEmail)]
		[DataRow(1, UserEmailOptions.PrivateMessagesFromAdmins)]
		[DataRow(2, UserEmailOptions.PrivateMessagesFromAll)]
		[TestMethod]
		public void Value(int expected, UserEmailOptions actual)
		{
			Assert.AreEqual(expected, (int)actual);
		}
	}
}
