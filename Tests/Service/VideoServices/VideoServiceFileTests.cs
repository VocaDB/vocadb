#nullable disable

using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices
{
	[TestClass]
	public class VideoServiceFileTests
	{
		private VideoServiceFile _videoService;

		private async Task TestGetVideoTitle(string url, string expected)
		{
			var actual = await _videoService.GetVideoTitleAsync(url);
			Assert.AreEqual(expected, actual.Title);
		}

		private void TestIsValidFor(string url, bool expected)
		{
			Assert.AreEqual(expected, _videoService.IsValidFor(url));
		}

		[TestInitialize]
		public void SetUp()
		{
			_videoService = new VideoServiceFile();
		}

		[TestMethod]
		public async Task GetVideoTitle_Simple()
		{
			await TestGetVideoTitle("http://caress.airtv.org/audio/car.ess - 2 UFO'r'IA.mp3", "car.ess - 2 UFO'r'IA.mp3");
		}

		[TestMethod]
		public async Task GetVideoTitle_WithParam()
		{
			await TestGetVideoTitle("http://caress.airtv.org/audio/car.ess - 2 UFO'r'IA.mp3?miku=39", "car.ess - 2 UFO'r'IA.mp3");
		}

		[TestMethod]
		public void IsValidFor_InvalidType()
		{
			TestIsValidFor("http://vocadb.net/miku.gif", false);
		}

		[TestMethod]
		public void IsValidFor_MatchExtension()
		{
			TestIsValidFor("http://www.mp3.com/", false); // Contains .mp3 but is not valid, because it's not a file extension.
		}

		[TestMethod]
		public void IsValidFor_Simple()
		{
			TestIsValidFor("http://vocadb.net/miku.mp3", true);
		}

		[TestMethod]
		public void IsValidFor_NoScheme()
		{
			TestIsValidFor("vocadb.net/miku.mp3", true);
		}

		// TODO: Not supported yet
		[Ignore]
		[TestMethod]
		public void IsValidFor_WithParam()
		{
			TestIsValidFor("http://vocadb.net/miku.mp3?miku=39", true);
		}
	}
}
