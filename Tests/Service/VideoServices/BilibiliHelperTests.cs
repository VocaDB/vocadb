using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices
{
	/// <summary>
	/// Unit tests for <see cref="VideoServiceBilibili"/>.
	/// </summary>
	[TestClass]
	public class VideoServiceBilibiliTests
	{
		VideoService _videoService = VideoServiceBilibili.Bilibili;

		[TestMethod]
		public void GetVideoId_Test()
		{
			_videoService.GetIdByUrl("https://bilibili.com/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://www.bilibili.com/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://bilibili.com/video/BV1KQ4y1i7b6").Should().Be("BV1KQ4y1i7b6");
			_videoService.GetIdByUrl("https://www.bilibili.com/video/BV1KQ4y1i7b6").Should().Be("BV1KQ4y1i7b6");
			_videoService.GetIdByUrl("https://acg.tv/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://www.acg.tv/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://bilibili.kankanews.com/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://www.bilibili.kankanews.com/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://bilibili.tv/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://www.bilibili.tv/video/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://b23.tv/BV1KQ4y1i7b6").Should().Be("BV1KQ4y1i7b6");
			_videoService.GetIdByUrl("https://www.b23.tv/BV1KQ4y1i7b6").Should().Be("BV1KQ4y1i7b6");
			_videoService.GetIdByUrl("https://b23.tv/av692583033").Should().Be("692583033");
			_videoService.GetIdByUrl("https://www.b23.tv/av692583033").Should().Be("692583033");
		}

		[TestMethod]
		public async Task ParseByUrlAsync_BvId()
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/BV17x411w7KC", false);

			result.IsOk.Should().BeTrue();
			result.Service.Should().Be(PVService.Bilibili);
			result.Id.Should().Be("170001");
		}

		[TestMethod]
		public async Task ParseByUrlAsync_BvId_IssueExample()
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/BV1TE411379f", false);

			result.IsOk.Should().BeTrue();
			result.Id.Should().Be("96216846");
		}

		[TestMethod]
		public async Task ParseByUrlAsync_AvId()
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/av692583033", false);

			result.IsOk.Should().BeTrue();
			result.Id.Should().Be("692583033");
		}

		[TestMethod]
		public async Task ParseByUrlAsync_InvalidBvId()
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/BV0000000000", false);

			result.IsOk.Should().BeFalse();
			result.ResultType.Should().Be(VideoUrlParseResultType.OtherError);
		}
	}
}
