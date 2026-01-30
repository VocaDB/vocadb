using System.Threading.Tasks;
using Xunit;

namespace VocaDb.BandcampMetadataExtractor.Tests
{
	public class BandcampMetadataClientTests
	{
		[Fact]
		public async Task ExtractAsync()
		{
			var client = new BandcampMetadataClient();

			var result = await client.ExtractAsync("http://youtube-dl.bandcamp.com/track/youtube-dl-test-song");
			Assert.Equal("1812978515", result.Id);
			Assert.Equal("youtube-dl  \"'/\\\u00e4\u21ad - youtube-dl  \"'/\\\u00e4\u21ad - youtube-dl test song \"'/\\\u00e4\u21ad", result.Title);
			Assert.Equal(9.8485, result.Duration);
			Assert.Equal("youtube-dl  \"'/\\\u00e4\u21ad", result.Uploader);
			//Assert.Equal(1354224127, result.Timestamp);
			Assert.Equal("20121129", result.UploadDate);

			result = await client.ExtractAsync("http://benprunty.bandcamp.com/track/lanius-battle");
			Assert.Equal("2650410135", result.Id);
			Assert.Equal("Ben Prunty - Lanius (Battle)", result.Title);
			Assert.Matches(@"^https?://.*\.jpg$", result.Thumbnail);
			Assert.Equal("Ben Prunty", result.Uploader);
			//Assert.Equal(1396508491, result.Timestamp);
			Assert.Equal("20140403", result.UploadDate);
			Assert.Equal(260.877, result.Duration);

			result = await client.ExtractAsync("https://relapsealumni.bandcamp.com/track/hail-to-fire");
			Assert.Equal("2584466013", result.Id);
			Assert.Equal("Mastodon - Hail to Fire", result.Title);
			Assert.Matches(@"^https?://.*\.jpg$", result.Thumbnail);
			Assert.Equal("Mastodon", result.Uploader);
			//Assert.Equal(1322005399, result.Timestamp);
			Assert.Equal("20111122", result.UploadDate);
			Assert.Equal(120.79, result.Duration);
		}
	}
}
