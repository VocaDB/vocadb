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
		[DataRow("BV1xx411c7mQ", "1")]
		[DataRow("BV1sZ411p7j9", "2147483647")]
		[DataRow("BV1Dr4U127Xc", "2147483648")]
		[DataRow("BV16t4Y1u78R", "10000000000")]
		[DataRow("BV1L9Uoa9EUx", "111298867365120")]
		[DataRow("BV1aPPTfmvQq", "2251799813685247")]
		public async Task ParseByUrlAsync_BvId_FullRange(string bvId, string aid)
		{
			var bvResult = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/" + bvId, false);
			var avResult = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/av" + aid, false);

			bvResult.IsOk.Should().BeTrue();
			bvResult.Service.Should().Be(PVService.Bilibili);
			bvResult.Id.Should().Be(aid);
			avResult.IsOk.Should().BeTrue();
			avResult.Id.Should().Be(bvResult.Id);
		}

		[TestMethod]
		[DataRow(3)]
		[DataRow(4)]
		[DataRow(5)]
		[DataRow(6)]
		[DataRow(7)]
		[DataRow(8)]
		[DataRow(9)]
		[DataRow(10)]
		[DataRow(11)]
		public async Task ParseByUrlAsync_BvId_InvalidCharacter(int position)
		{
			var characters = "BV17x411w7KC".ToCharArray();
			characters[position] = '_';
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/" + new string(characters), false);

			result.IsOk.Should().BeFalse();
			result.ResultType.Should().Be(VideoUrlParseResultType.OtherError);
		}

		[TestMethod]
		[DataRow("av0")]
		[DataRow("av2251799813685248")]
		[DataRow("av9223372036854775808")]
		public async Task ParseByUrlAsync_InvalidAvId(string avId)
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/" + avId, false);

			result.IsOk.Should().BeFalse();
			result.ResultType.Should().Be(VideoUrlParseResultType.OtherError);
		}

		[TestMethod]
		[DataRow("BV0000000000")]
		[DataRow("BV07x411w7KC")]
		[DataRow("BV27x411w7KC")]
		[DataRow("BV17x411w7K")]
		[DataRow("BV17x411w7KCC")]
		[DataRow("BV1xx411c7mX")]
		[DataRow("BV1FFFFFFFFF")]
		[DataRow("BV1fffffffff")]
		public async Task ParseByUrlAsync_InvalidBvId(string bvId)
		{
			var result = await _videoService.ParseByUrlAsync("https://www.bilibili.com/video/" + bvId, false);

			result.IsOk.Should().BeFalse();
			result.ResultType.Should().Be(VideoUrlParseResultType.OtherError);
		}
	}
}
