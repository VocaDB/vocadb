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
			_videoService.GetIdByUrl("https://bilibili.com/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://www.bilibili.com/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://bilibili.com/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://www.bilibili.com/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://bilibili.com/en/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://www.bilibili.com/en/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://bilibili.com/zh-Hans/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://www.bilibili.com/zh-Hans/video/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://acg.tv/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://www.acg.tv/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://bilibili.kankanews.com/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://www.bilibili.kankanews.com/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://bilibili.tv/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://www.bilibili.tv/video/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://b23.tv/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://www.b23.tv/BV1bH4B6dEuy").Should().Be("BV1bH4B6dEuy");
			_videoService.GetIdByUrl("https://b23.tv/av54139747").Should().Be("54139747");
			_videoService.GetIdByUrl("https://www.b23.tv/av54139747").Should().Be("54139747");
		}


	}
}
