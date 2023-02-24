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


	}
}
