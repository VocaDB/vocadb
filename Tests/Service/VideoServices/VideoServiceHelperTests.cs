#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Service.VideoServices;

namespace VocaDb.Tests.Service.VideoServices
{
	/// <summary>
	/// Tests for <see cref="VideoServiceHelper"/>.
	/// </summary>
	[TestClass]
	public class VideoServiceHelperTests
	{
		private PVForSong _originalWithThumb;
		private PVForSong _reprintWithThumb;

		[TestInitialize]
		public void SetUp()
		{
			_originalWithThumb = new PVForSong { Service = PVService.NicoNicoDouga, PVType = PVType.Original, PVId = "original_id", ThumbUrl = "original" };
			_reprintWithThumb = new PVForSong { Service = PVService.Youtube, PVType = PVType.Reprint, PVId = "reprint_id", ThumbUrl = "reprint" };
		}

		[TestMethod]
		public void GetThumbUrl_Nothing()
		{
			var result = VideoServiceHelper.GetThumbUrl(new IPVWithThumbnail[0]);

			result.Should().Be(string.Empty, "result");
		}

		[TestMethod]
		public void GetThumbUrl_HasOriginalWithThumb()
		{
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("original", "result");
		}

		[TestMethod]
		public void GetThumbUrl_HasOnlyReprint()
		{
			var pvs = new[] { _reprintWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("reprint", "result");
		}

		[TestMethod]
		public void GetThumbUrl_HasReprintWithThumb()
		{
			_originalWithThumb.ThumbUrl = string.Empty;
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("reprint", "result");
		}

		[TestMethod]
		public void GetThumbUrl_HasOtherWithThumb()
		{
			_originalWithThumb.ThumbUrl = string.Empty;
			_reprintWithThumb.PVType = PVType.Other;
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("reprint", "result");
		}

		/// <summary>
		/// PV Id with default thumbnail path will be used as fallback when there's no PVs with thumbnail URL.
		/// </summary>
		[TestMethod]
		public void GetThumbUrl_HasNoThumbs()
		{
			_originalWithThumb.ThumbUrl = string.Empty;
			_reprintWithThumb.ThumbUrl = string.Empty;
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };
			var nicoThumb = VideoService.NicoNicoDouga.GetThumbUrlById(_originalWithThumb.PVId);

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be(nicoThumb, "result");
		}

		[TestMethod]
		public void GetThumbUrl_Disabled()
		{
			_originalWithThumb.Disabled = true;
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("reprint", "result");
		}

		[TestMethod]
		public void GetThumbUrl_OnlyDisabledWithThumb()
		{
			_originalWithThumb.ThumbUrl = string.Empty;
			_reprintWithThumb.Disabled = true;
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrl(pvs);

			result.Should().Be("reprint", "result");
		}

		[TestMethod]
		public void GetThumbUrl_PreferNotNico()
		{
			var pvs = new[] { _reprintWithThumb, _originalWithThumb };

			var result = VideoServiceHelper.GetThumbUrlPreferNotNico(pvs);

			result.Should().Be("reprint", "result");
		}
	}
}
