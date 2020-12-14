#nullable disable

using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers
{
	/// <summary>
	/// Tests for <see cref="EnumerableExtender"/>.
	/// </summary>
	[TestClass]
	public class EnumerableExtenderTests
	{
		[TestMethod]
		public void MinOrNull_NotEmpty()
		{
			Assert.AreEqual(3, new int[] { 39, 3, 9 }.MinOrNull());
		}

		[TestMethod]
		public void MinOrNull_Empty_NotNullable()
		{
			Assert.IsNull(new int[0].MinOrNull());
		}
	}
}
