using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices {

	/// <summary>
	/// Tests for <see cref="VideoServiceHelper"/>.
	/// </summary>
	[TestClass]
	public class VideoServiceHelperTests {

		private PVForSong originalWithThumb;
		private PVForSong reprintWithThumb;

		[TestInitialize]
		public void SetUp() {
			
			originalWithThumb = new PVForSong { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "original_id", ThumbUrl = "original" };
			reprintWithThumb = new PVForSong { Service = PVService.Youtube, PVType = PVType.Reprint, PVId = "reprint_id", ThumbUrl = "reprint" };

		}

		[TestMethod]
		public void GetThumbUrl_Nothing() {

			var result = VideoServiceHelper.GetThumbUrl(new IPVWithThumbnail[0]);

			Assert.AreEqual(string.Empty, result, "result");

		}

		[TestMethod]
		public void GetThumbUrl_HasOriginalWithThumb() {

			var pvs = new[] {reprintWithThumb, originalWithThumb};

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			Assert.AreEqual("original", result, "result");

		}

		[TestMethod]
		public void GetThumbUrl_HasOnlyReprint() {

			var pvs = new[] { reprintWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			Assert.AreEqual("reprint", result, "result");

		}

		[TestMethod]
		public void GetThumbUrl_HasReprintWithThumb() {

			originalWithThumb.ThumbUrl = string.Empty;
			var pvs = new[] { reprintWithThumb, originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			Assert.AreEqual("reprint", result, "result");

		}

		/// <summary>
		/// PV Id with default thumbnail path will be used as fallback when there's no PVs with thumbnail URL.
		/// </summary>
		[TestMethod]
		public void GetThumbUrl_HasNoThumbs() {

			originalWithThumb.ThumbUrl = string.Empty;
			reprintWithThumb.ThumbUrl = string.Empty;
			var pvs = new[] { reprintWithThumb, originalWithThumb };
			var nicoThumb = VideoService.NicoNicoDouga.GetThumbUrlById(originalWithThumb.PVId);

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			Assert.AreEqual(nicoThumb, result, "result");

		}

		[TestMethod]
		public void GetThumbUrl_Disabled() {

			originalWithThumb.Disabled = true;
			var pvs = new[] { reprintWithThumb, originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			Assert.AreEqual("reprint", result, "result");

		}

		[TestMethod]
		public void GetThumbUrl_PreferNotNico() {

			var pvs = new[] { reprintWithThumb, originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrlPreferNotNico(pvs);

			Assert.AreEqual("reprint", result, "result");

		}

	}

}
