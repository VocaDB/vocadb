#nullable disable

using System.Drawing;
using System.IO;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Images
{
	/// <summary>
	/// Tests for <see cref="ImageThumbGenerator"/>.
	/// </summary>
	[TestClass]
	public class ImageThumbGeneratorTests
	{
		private InMemoryImagePersister _persister;
		private ImageThumbGenerator _target;

		private Stream TestImage() => ResourceHelper.TestImage();

		private void AssertDimensions(IEntryImageInformation imageInfo, ImageSize size, int width, int height)
		{
			using (var stream = _persister.GetReadStream(imageInfo, size))
			using (var img = Image.FromStream(stream))
			{
				img.Width.Should().Be(width, "Image width");
				img.Height.Should().Be(height, "Image height");
			}
		}

		private EntryThumbContract CallGenerateThumbsAndMoveImage(ImageSizes sizes)
		{
			var thumb = new EntryThumbContract { EntryType = EntryType.SongList };

			using (var input = TestImage())
			{
				_target.GenerateThumbsAndMoveImage(input, thumb, sizes);
			}

			return thumb;
		}

		[TestInitialize]
		public void SetUp()
		{
			_persister = new InMemoryImagePersister();
			_target = new ImageThumbGenerator(_persister);
		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Original()
		{
			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Original);

			_persister.HasImage(thumb, ImageSize.Original).Should().BeTrue("Image was created");
			AssertDimensions(thumb, ImageSize.Original, 480, 800);
		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Thumbnail()
		{
			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Thumb);

			_persister.HasImage(thumb, ImageSize.Thumb).Should().BeTrue("Image was created");
			AssertDimensions(thumb, ImageSize.Thumb, 150, 250);
		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_OriginalAndSmallThumb()
		{
			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Original | ImageSizes.SmallThumb);

			_persister.HasImage(thumb, ImageSize.Original).Should().BeTrue("Image was created");
			AssertDimensions(thumb, ImageSize.Original, 480, 800);
			_persister.HasImage(thumb, ImageSize.SmallThumb).Should().BeTrue("Thumbnail was created");
			AssertDimensions(thumb, ImageSize.SmallThumb, 90, 150);
		}
	}
}
