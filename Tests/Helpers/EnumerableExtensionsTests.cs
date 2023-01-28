#nullable disable

using VocaDb.Model.Helpers;

namespace VocaDb.Tests.Helpers;

/// <summary>
/// Tests for <see cref="EnumerableExtensions"/>.
/// </summary>
[TestClass]
public class EnumerableExtensionsTests
{
	[TestMethod]
	public void MinOrNull_NotEmpty()
	{
		new int[] { 39, 3, 9 }.MinOrNull().Should().Be(3);
	}

	[TestMethod]
	public void MinOrNull_Empty_NotNullable()
	{
		Array.Empty<int>().MinOrNull().Should().BeNull();
	}
}
