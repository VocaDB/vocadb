#nullable disable

using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices;

/// <summary>
/// Tests for <see cref="SoundCloudId"/>.
/// </summary>
[TestClass]
public class SoundCloudIdTests
{
	[TestMethod]
	public void Ctor_QueryParams()
	{
		var result = new SoundCloudId("3939", "voicetextactor/original-demo-song-ver2017-07-6?in=voicetextactor/sets/all-1");

		result.SoundCloudUrl.Should().Be("voicetextactor/original-demo-song-ver2017-07-6");
	}
}
