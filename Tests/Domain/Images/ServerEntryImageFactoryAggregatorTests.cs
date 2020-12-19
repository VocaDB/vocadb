#nullable disable

using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VocaDb.Model.DataContracts.Albums;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Images;
using VocaDb.Tests.TestData;
using VocaDb.Tests.TestSupport;

namespace VocaDb.Tests.Domain.Images
{
	/// <summary>
	/// Tests for <see cref="ServerEntryImageFactoryAggregator"/>.
	/// </summary>
	[TestClass]
	public class ServerEntryImageFactoryAggregatorTests
	{
		public ServerEntryImageFactoryAggregatorTests()
		{
			_dynamicImageUrlFactory = new FakeDynamicImageUrlFactory();
			_imageStore = new InMemoryImagePersisterStore();
			_urlFactory = new ServerEntryImageFactoryAggregator(_dynamicImageUrlFactory, new InMemoryEntryThumbPersister(_imageStore));

			var album = CreateEntry.Album(1, coverPictureMime: "image/png");
			_albumContract = AlbumContract(album);
			AddImage(_albumContract, ImageSize.Thumb);
		}

		private readonly AlbumContract _albumContract;
		private readonly FakeDynamicImageUrlFactory _dynamicImageUrlFactory;
		private readonly ServerEntryImageFactoryAggregator _urlFactory;
		private readonly InMemoryImagePersisterStore _imageStore;

		private void AddImage(IEntryImageInformation imageInfo, ImageSize size)
		{
			using (var stream = ResourceHelper.TestImage())
			{
				_imageStore.Write(imageInfo, size, stream);
			}
		}

		private void AssertImageDatabase(VocaDbUrl url, IEntryImageInformation imageInfo, ImageSize size, string because = "")
		{
			url.Should().Be(_dynamicImageUrlFactory.GetUrl(imageInfo, size), because);
		}

		private void AssertImageFileSystem(VocaDbUrl url, IEntryImageInformation imageInfo, ImageSize size, string because = "")
		{
			url.Should().Be(_imageStore.GetUrl(imageInfo, size), because);
		}

		private AlbumContract AlbumContract(Album album) => new AlbumContract(album, ContentLanguagePreference.Default);

		[TestMethod]
		public void GetUrl_AlbumOriginalImageExistsInDatabase()
		{
			var url = _urlFactory.GetUrl(_albumContract, ImageSize.Original);
			AssertImageDatabase(url, _albumContract, ImageSize.Original, because: "Original album image always loaded from database");
		}

		[TestMethod]
		public void GetUrl_AlbumThumbnailImageExistsOnDisk()
		{
			var url = _urlFactory.GetUrl(_albumContract, ImageSize.Thumb);
			AssertImageFileSystem(url, _albumContract, ImageSize.Thumb, because: "Album thumbnail loaded from disk if possible");
		}

		[TestMethod]
		public void GetUrl_AlbumThumbnailImageExistsInDatabase()
		{
			var albumContract2 = AlbumContract(CreateEntry.Album(2, "Another Dimensions", coverPictureMime: "image/png"));
			var url = _urlFactory.GetUrl(albumContract2, ImageSize.Thumb);
			AssertImageDatabase(url, albumContract2, ImageSize.Thumb, because: "Album thumbnail loaded from database if missing from disk");
		}

		[TestMethod]
		public void GetUrl_AlbumOriginalImageMissing()
		{
			var albumContract2 = AlbumContract(CreateEntry.Album(2, "Another Dimensions"));
			var url = _urlFactory.GetUrl(albumContract2, ImageSize.Original);
			AssertImageDatabase(url, albumContract2, ImageSize.Original, because: "Original album image always loaded from database");
		}

		[TestMethod]
		public void GetUrl_AlbumOriginalThumbnailMissing()
		{
			var albumContract2 = AlbumContract(CreateEntry.Album(2, "Another Dimensions"));
			var url = _urlFactory.GetUrl(albumContract2, ImageSize.Thumb);
			AssertImageFileSystem(url, albumContract2, ImageSize.Thumb, because: "Missing thumbnail attempted to be loaded from primary location on disk");
		}

		[TestMethod]
		public void GetUrl_TagOriginalImageExistsOnDisk()
		{
			var tag = CreateEntry.Tag("trance", pictureMime: "image/png");
			AddImage(tag.Thumb, ImageSize.Original);
			var url = _urlFactory.GetUrl(tag.Thumb, ImageSize.Original);
			AssertImageFileSystem(url, tag.Thumb, ImageSize.Original, because: "Tag images are always on disk");
		}

		[TestMethod]
		public void GetUrl_TagOriginalImageMissingFromDisk()
		{
			var tag = CreateEntry.Tag("trance", pictureMime: "image/png");
			var url = _urlFactory.GetUrl(tag.Thumb, ImageSize.Original);
			AssertImageFileSystem(url, tag.Thumb, ImageSize.Original, because: "Tag images are always on disk");
		}
	}
}
