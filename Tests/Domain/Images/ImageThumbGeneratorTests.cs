using System.Drawing;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Images {

	/// <summary>
	/// Tests for <see cref="ImageThumbGenerator"/>.
	/// </summary>
	[TestClass]
	public class ImageThumbGeneratorTests {

		private InMemoryImagePersister persister;
		private ImageThumbGenerator target;

		private Stream TestImage() {
			return ResourceHelper.GetFileStream("yokohma_bay_concert.jpg");
		}

		private void AssertDimensions(IEntryImageInformation imageInfo, ImageSize size, int width, int height) {

			using (var stream = persister.GetReadStream(imageInfo, size))
			using (var img = Image.FromStream(stream)) {
				Assert.AreEqual(width, img.Width, "Image width");
				Assert.AreEqual(height, img.Height, "Image height");
			}

		}

		private EntryThumbContract CallGenerateThumbsAndMoveImage(ImageSizes sizes) {

			var thumb = new EntryThumbContract {EntryType = EntryType.SongList, FileName = "test.jpg"};

			using (var input = TestImage()) {
				target.GenerateThumbsAndMoveImage(input, thumb, sizes);				
			}
			
			return thumb;

		}

		[TestInitialize]
		public void SetUp() {

			persister = new InMemoryImagePersister();
			target = new ImageThumbGenerator(persister);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Original() {

			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Original);

			Assert.IsTrue(persister.HasImage(thumb, ImageSize.Original), "Image was created");
			AssertDimensions(thumb, ImageSize.Original, 480, 800);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_Thumbnail() {

			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Thumb);

			Assert.IsTrue(persister.HasImage(thumb, ImageSize.Thumb), "Image was created");
			AssertDimensions(thumb, ImageSize.Thumb, 150, 250);

		}

		[TestMethod]
		public void GenerateThumbsAndMoveImage_OriginalAndSmallThumb() {

			var thumb = CallGenerateThumbsAndMoveImage(ImageSizes.Original | ImageSizes.SmallThumb);

			Assert.IsTrue(persister.HasImage(thumb, ImageSize.Original), "Image was created");
			AssertDimensions(thumb, ImageSize.Original, 480, 800);
			Assert.IsTrue(persister.HasImage(thumb, ImageSize.SmallThumb), "Thumbnail was created");
			AssertDimensions(thumb, ImageSize.SmallThumb, 90, 150);

		}

	}

}
