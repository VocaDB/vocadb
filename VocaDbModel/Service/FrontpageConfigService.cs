using Amazon.S3;
using Amazon.S3.Model;
using System.Configuration;
using VocaDb.Model.Database.Queries;
using VocaDb.Model.DataContracts;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;

namespace VocaDb.Model.Service;

public sealed class FrontpageConfigService
{
	private readonly FrontpageConfigQueries _queries;
	private readonly IUserPermissionContext _permissionContext;
	private readonly IAmazonS3? _s3Client;
	private readonly string _s3BucketName;

	public FrontpageConfigService(
		FrontpageConfigQueries queries,
		IUserPermissionContext permissionContext)
	{
		_queries = queries;
		_permissionContext = permissionContext;

		// Initialize S3 client with same config as S3EntryImagePersister
		var awsAccessKey = ConfigurationManager.AppSettings["S3AccessKey"] ?? string.Empty;
		var awsSecretKey = ConfigurationManager.AppSettings["S3SecretKey"] ?? string.Empty;
		_s3BucketName = ConfigurationManager.AppSettings["S3BucketName"] ?? string.Empty;
		var awsEndpoint = ConfigurationManager.AppSettings["S3Endpoint"] ?? string.Empty;

		if (!string.IsNullOrEmpty(_s3BucketName))
		{
			var config = new AmazonS3Config
			{
				ServiceURL = string.IsNullOrEmpty(awsEndpoint) ? null : awsEndpoint,
				ForcePathStyle = true,
				UseHttp = false,
				DisableHostPrefixInjection = true
			};

			if (!string.IsNullOrEmpty(awsAccessKey) && !string.IsNullOrEmpty(awsSecretKey))
				_s3Client = new AmazonS3Client(awsAccessKey, awsSecretKey, config);
			else
				_s3Client = new AmazonS3Client(config);
		}
	}

	public FrontpageConfigContract GetConfig()
	{
		return _queries.GetFrontpageConfig();
	}

	public void UpdateConfig(FrontpageConfigContract config)
	{
		_queries.UpdateFrontpageConfig(config);
	}

	public string UploadBannerImage(Stream stream, string originalFileName, string contentType)
	{
		_permissionContext.VerifyPermission(PermissionToken.Admin);

		if (stream == null || stream.Length == 0)
			throw new ArgumentException("No file provided");

		var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
		var ext = Path.GetExtension(originalFileName).ToLowerInvariant();

		if (!allowedExtensions.Contains(ext))
			throw new ArgumentException("Invalid file type. Allowed types: jpg, png, gif, webp");

		// Max 5MB
		if (stream.Length > 5 * 1024 * 1024)
			throw new ArgumentException("File too large. Maximum size is 5MB");

		// Validate image
		stream.Position = 0;
		var original = ImageHelper.OpenImage(stream);
		if (original == null)
			throw new ArgumentException("Invalid image file");

		// Generate unique filename
		var fileName = $"banner_{Guid.NewGuid()}{ext}";

		// Upload to S3 if configured, otherwise save locally
		if (_s3Client != null && !string.IsNullOrEmpty(_s3BucketName))
		{
			UploadToS3(stream, fileName, contentType);
		}
		else
		{
			UploadLocally(stream, fileName);
		}

		return fileName;
	}

	private void UploadToS3(Stream stream, string fileName, string contentType)
	{
		var s3Key = $"banners/{fileName}";

		stream.Position = 0;
		using var memoryStream = new MemoryStream();
		stream.CopyTo(memoryStream);
		memoryStream.Position = 0;

		var request = new PutObjectRequest
		{
			BucketName = _s3BucketName,
			Key = s3Key,
			InputStream = memoryStream,
			ContentType = contentType ?? "application/octet-stream",
			UseChunkEncoding = false,
			ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
			DisableDefaultChecksumValidation = true
		};

		try
		{
			_s3Client!.PutObjectAsync(request).GetAwaiter().GetResult();
		}
		catch (AmazonS3Exception ex)
		{
			throw new InvalidOperationException($"Failed to upload banner image to S3: {ex.Message}", ex);
		}
	}

	private void UploadLocally(Stream stream, string fileName)
	{
		var targetDir = Path.Combine(
			AppDomain.CurrentDomain.BaseDirectory,
			"Content",
			"banners"
		);

		Directory.CreateDirectory(targetDir);

		var targetPath = Path.Combine(targetDir, fileName);

		stream.Position = 0;
		using var fileStream = new FileStream(targetPath, FileMode.Create);
		stream.CopyTo(fileStream);
	}
}
