using VocaDb.Model.Domain.PVs;

namespace VocaDb.Tests.Domain.PVs;

[TestClass]
public class PVTests
{
	[TestMethod]
	public void Properties_RemoveInvalidXmlChars()
	{
		var pv = new PV
		{
			Name = "a\u0008b",
			Author = "c\u000Bd",
			Description = "e\u001Ff",
		};

		pv.Name.Should().Be("ab");
		pv.Author.Should().Be("cd");
		pv.Description.Should().Be("ef");
	}
}
