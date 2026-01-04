#nullable disable

using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using SixLabors.ImageSharp.Formats;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Domain.Images
{
    /// <summary>
    /// Persister that stores entry images in an S3-compatible bucket (Tigris S3).
    /// Reads AWS/S3 settings from configuration keys: S3AccessKey, S3SecretKey, S3BucketName, S3Endpoint.
    /// </summary>
    public class S3EntryImagePersister : IEntryThumbPersister, IEntryPictureFilePersister
    {
        private readonly string _awsAccessKey;
        private readonly string _awsSecretKey;
        private readonly string _awsBucketName;
        private readonly string _awsEndpoint;
        private readonly IAmazonS3 _s3Client;

        private static string GetDir(ImageSize size) => size switch
        {
            ImageSize.Original => "Orig",
            ImageSize.Thumb => "Thumb",
            ImageSize.SmallThumb => "Small",
            ImageSize.TinyThumb => "Tiny",
            _ => throw new NotSupportedException(),
        };

        private string GetKey(IEntryImageInformation picture, ImageSize size)
        {
            return $"img/{picture.EntryType.ToString()}/{picture.Purpose.ToString().ToLowerInvariant()}{GetDir(size)}/{picture.Id}{ImageHelper.GetExtensionFromMime(picture.Mime)}";
        }

        public S3EntryImagePersister()
        {
            _awsAccessKey = ConfigurationManager.AppSettings["S3AccessKey"] ?? string.Empty;
            _awsSecretKey = ConfigurationManager.AppSettings["S3SecretKey"] ?? string.Empty;
            _awsBucketName = ConfigurationManager.AppSettings["S3BucketName"] ?? string.Empty;
            _awsEndpoint = ConfigurationManager.AppSettings["S3Endpoint"] ?? string.Empty;

            if (string.IsNullOrEmpty(_awsBucketName))
                return;

            var config = new AmazonS3Config
            {
                ServiceURL = string.IsNullOrEmpty(_awsEndpoint) ? null : _awsEndpoint,
                ForcePathStyle = true,
                UseHttp = false,
                DisableHostPrefixInjection = true
            };

            if (!string.IsNullOrEmpty(_awsAccessKey) && !string.IsNullOrEmpty(_awsSecretKey))
                _s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, config);
            else
                _s3Client = new AmazonS3Client(config);
        }

        public Stream GetReadStream(IEntryImageInformation picture, ImageSize size)
        {
            if (_s3Client == null) return new MemoryStream();

            var key = GetKey(picture, size);
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = _awsBucketName,
                    Key = key
                };
                var response = _s3Client.GetObjectAsync(request).Result;
                var ms = new MemoryStream();
                response.ResponseStream.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }
            catch (AggregateException ae) when (ae.InnerException is AmazonS3Exception s3ex && s3ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new MemoryStream();
            }
            catch (AmazonS3Exception)
            {
                return new MemoryStream();
            }
            catch (Exception)
            {
                return new MemoryStream();
            }
        }

        public void Write(IEntryImageInformation picture, ImageSize size, Stream stream)
        {
            if (_s3Client == null) return;

            var key = GetKey(picture, size);

            stream.Position = 0;
            var buffer = new byte[stream.Length];
            stream.Read(buffer, 0, buffer.Length);

            using var memoryStream = new MemoryStream(buffer);

            var request = new PutObjectRequest
            {
                BucketName = _awsBucketName,
                Key = key,
                InputStream = memoryStream,
                ContentType = picture.Mime ?? "application/octet-stream",
                UseChunkEncoding = false,
                ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
                DisableDefaultChecksumValidation = true
            };

            try
            {
                var response = _s3Client.PutObjectAsync(request).GetAwaiter().GetResult();
            }
            catch (AmazonS3Exception ex)
            {
                throw new InvalidOperationException($"Failed to upload image to S3: {ex.Message}", ex);
            }
        }

        public void Write(IEntryImageInformation picture, ImageSize size, Image image)
        {
            if (_s3Client == null) return;

            using var stream = new MemoryStream();
            IImageFormat format = image.Metadata.DecodedImageFormat ?? SixLabors.ImageSharp.Formats.Png.PngFormat.Instance;

            try
            {
                image.Save(stream, format);

                if (stream.Length == 0)
                {
                    throw new InvalidOperationException("Image stream is empty after encoding");
                }

                stream.Position = 0;

                var actualMime = GetMimeTypeFromFormat(format);
                var pictureWithCorrectMime = new ImageInformationWrapper(picture, actualMime);

                Write(pictureWithCorrectMime, size, stream);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save image to S3", ex);
            }
        }

        private static string GetMimeTypeFromFormat(IImageFormat format)
        {
            return format.Name.ToLowerInvariant() switch
            {
                "jpeg" => "image/jpeg",
                "jpg" => "image/jpeg",
                "png" => "image/png",
                "gif" => "image/gif",
                "webp" => "image/webp",
                "bmp" => "image/bmp",
                _ => "image/png"
            };
        }

        private class ImageInformationWrapper : IEntryImageInformation
        {
            private readonly IEntryImageInformation _original;
            private readonly string _mime;

            public ImageInformationWrapper(IEntryImageInformation original, string mime)
            {
                _original = original;
                _mime = mime;
            }

            public int Id => _original.Id;
            public EntryType EntryType => _original.EntryType;
            public ImagePurpose Purpose => _original.Purpose;
            public string Mime => _mime;
            public int Version => _original.Version;
        }

        public VocaDbUrl GetUrl(IEntryImageInformation picture, ImageSize size)
        {
            var key = GetKey(picture, size);
            var url = (picture.Version > 0)
                ? $"{key}?v={picture.Version}"
                : key;

            return new VocaDbUrl(url, UrlDomain.Static, UriKind.Relative);
        }

        public bool HasImage(IEntryImageInformation picture, ImageSize size)
        {
            return IsSupported(picture, size) && picture.ShouldExist();
        }

        public bool IsSupported(IEntryImageInformation picture, ImageSize size) =>
            picture.EntryType == EntryType.ReleaseEvent ||
            picture.EntryType == EntryType.ReleaseEventSeries ||
            picture.EntryType == EntryType.SongList ||
            picture.EntryType == EntryType.Tag ||
            (
                (picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album) &&
                picture.PurposeMainOrUnspecified()
            ) ||
            (
                (picture.EntryType == EntryType.Artist || picture.EntryType == EntryType.Album) &&
                picture.Purpose == ImagePurpose.Additional
            ) ||
            picture.EntryType == EntryType.User;

        public string GetPath(IEntryImageInformation picture, ImageSize size)
        {
            return GetKey(picture, size);
        }
    }
}
