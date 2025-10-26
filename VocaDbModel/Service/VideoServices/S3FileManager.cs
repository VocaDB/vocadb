#nullable disable

using System.Configuration;
using System.Security.Principal;
using Amazon.S3;
using Amazon.S3.Model;
using NLog;
using TagLib;
using VocaDb.Model.DataContracts.PVs;
using VocaDb.Model.Domain.PVs;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Web;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;
using SixLabors.ImageSharp;
using File = System.IO.File;

namespace VocaDb.Model.Service.VideoServices;

public class S3FileManager
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	public const int MaxMediaSizeMB = 20;
	public const int MaxMediaSizeBytes = MaxMediaSizeMB * 1024 * 1024;
	public static readonly string[] Extensions = { ".mp3", ".jpg", ".png" };
	public static readonly string[] MimeTypes = { "audio/mp3", "audio/mpeg", "image/jpeg", "image/png" };

	private readonly string _awsAccessKey;
	private readonly string _awsSecretKey;
	private readonly string _awsBucketName;
	private readonly string _awsEndpoint;
	private readonly IAmazonS3 _s3Client;

	public static bool IsAudio(string filename)
	{
		return !IsImage(filename);
	}

	public static bool IsImage(string filename)
	{
		string[] imageExtensions = { ".jpg", ".png" };
		var ext = Path.GetExtension(filename);
		return imageExtensions.Contains(ext);
	}

	public S3FileManager()
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
			UseHttp = false, // Force HTTPS
			DisableHostPrefixInjection = true, // Disable host prefix injection
			BufferSize = 8192 // Set explicit buffer size
		};

		if (!string.IsNullOrEmpty(_awsAccessKey) && !string.IsNullOrEmpty(_awsSecretKey))
			_s3Client = new AmazonS3Client(_awsAccessKey, _awsSecretKey, config);
		else
			_s3Client = new AmazonS3Client(config);
	}

	public PVContract CreatePVContract(IHttpPostedFile file, IIdentity user, IUser loggedInUser)
	{
		var tempFile = Path.ChangeExtension(Path.GetTempFileName(), ImageHelper.GetExtensionFromMime(file.ContentType));
		file.SaveAs(tempFile);

		var filename = Path.GetFileName(tempFile);
		var pv = new PVContract { Service = PVService.LocalFile, PVId = filename };

		using (var mp3 = TagLib.File.Create(tempFile, file.ContentType, ReadStyle.Average))
		{
			pv.Name = mp3.Tag.Title;
			pv.Author = user.Name;
			pv.Length = (int)mp3.Properties.Duration.TotalSeconds;
		}

		pv.CreatedBy = loggedInUser.Id;

		if (string.IsNullOrEmpty(pv.Name))
			pv.Name = Path.GetFileNameWithoutExtension(file.FileName);

		return pv;
	}

	private string GetS3Key(string pvId)
	{
		return $"media/{pvId}";
	}

	private string GetS3ThumbKey(string pvId)
	{
		return $"media-thumb/{pvId}";
	}

	private async Task<bool> UploadFileToS3Async(string filePath, string s3Key, string contentType)
	{
		if (_s3Client == null) return false;

		try
		{
			using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			
			var request = new PutObjectRequest
			{
				BucketName = _awsBucketName,
				Key = s3Key,
				InputStream = fileStream,
				ContentType = contentType ?? "application/octet-stream",
				UseChunkEncoding = false,
				ServerSideEncryptionMethod = ServerSideEncryptionMethod.None,
				DisableDefaultChecksumValidation = true
			};

			var response = await _s3Client.PutObjectAsync(request);
			return response.HttpStatusCode == System.Net.HttpStatusCode.OK;
		}
		catch (AmazonS3Exception ex)
		{
			s_log.Error(ex, "Failed to upload file to S3: " + s3Key);
			return false;
		}
		catch (Exception ex)
		{
			s_log.Error(ex, "Unexpected error uploading file to S3: " + s3Key);
			return false;
		}
	}

	private async Task<bool> DeleteFileFromS3Async(string s3Key)
	{
		if (_s3Client == null) return false;

		try
		{
			var request = new DeleteObjectRequest
			{
				BucketName = _awsBucketName,
				Key = s3Key
			};

			var response = await _s3Client.DeleteObjectAsync(request);
			return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent || 
			       response.HttpStatusCode == System.Net.HttpStatusCode.OK;
		}
		catch (AmazonS3Exception ex)
		{
			s_log.Error(ex, "Failed to delete file from S3: " + s3Key);
			return false;
		}
		catch (Exception ex)
		{
			s_log.Error(ex, "Unexpected error deleting file from S3: " + s3Key);
			return false;
		}
	}

	private async Task CreateThumbnailAsync(string tempFilePath, string pvId, PVForSong pv)
	{
		if (!IsImage(tempFilePath))
			return;

		try
		{
			using var stream = new FileStream(tempFilePath, FileMode.Open);
			using var original = ImageHelper.OpenImage(stream);
			var thumb = ImageHelper.ResizeToFixedSize(original, 560, 315);
			
			// Save thumbnail to temp file first
			var tempThumbPath = Path.ChangeExtension(Path.GetTempFileName(), Path.GetExtension(tempFilePath));
			thumb.Save(tempThumbPath);

			// Upload thumbnail to S3
			var thumbKey = GetS3ThumbKey(pvId);
			var contentType = GetContentTypeFromExtension(Path.GetExtension(tempFilePath));
			
			if (await UploadFileToS3Async(tempThumbPath, thumbKey, contentType))
			{
				pv.ThumbUrl = GetS3Url(thumbKey);
				pv.Song.UpdateThumbUrl();
			}

			// Clean up temp thumbnail file
			try
			{
				File.Delete(tempThumbPath);
			}
			catch (IOException ex)
			{
				s_log.Warn(ex, "Unable to delete temporary thumbnail file: " + tempThumbPath);
			}
		}
		catch (Exception ex)
		{
			s_log.Error(ex, "Failed to create and upload thumbnail for: " + pvId);
		}
	}

	private string GetContentTypeFromExtension(string extension)
	{
		return extension.ToLowerInvariant() switch
		{
			".mp3" => "audio/mpeg",
			".jpg" => "image/jpeg",
			".jpeg" => "image/jpeg",
			".png" => "image/png",
			_ => "application/octet-stream"
		};
	}

	private string GetS3Url(string s3Key)
	{
		if (!string.IsNullOrEmpty(_awsEndpoint))
		{
			var endpoint = _awsEndpoint.TrimEnd('/');
			return string.IsNullOrEmpty(_awsBucketName) ? $"{endpoint}/{s3Key}" : $"{endpoint}/{_awsBucketName}/{s3Key}";
		}
		
		return $"/s3/{s3Key}";
	}

	public async Task SyncS3FilePVsAsync(CollectionDiff<PVForSong, PVForSong> diff, int songId)
	{
		var addedLocalMedia = diff.Added.Where(m => m.Service == PVService.LocalFile);
		foreach (var pv in addedLocalMedia)
		{
			var oldFull = Path.Combine(Path.GetTempPath(), pv.PVId);

			if (Path.GetDirectoryName(oldFull) != Path.GetDirectoryName(Path.GetTempPath()))
				throw new InvalidOperationException("File folder doesn't match with temporary folder");

			if (!Extensions.Contains(Path.GetExtension(oldFull)))
				throw new InvalidOperationException("Invalid extension");

			var newId = $"{pv.Author}-S{songId}-{pv.PVId}";
			var s3Key = GetS3Key(newId);
			pv.PVId = newId;

			try
			{
				// Upload file to S3
				var contentType = GetContentTypeFromExtension(Path.GetExtension(oldFull));
				if (await UploadFileToS3Async(oldFull, s3Key, contentType))
				{
					// Create thumbnail if it's an image
					await CreateThumbnailAsync(oldFull, newId, pv);
					
					// Delete temp file after successful upload
					try
					{
						File.Delete(oldFull);
					}
					catch (IOException ex)
					{
						s_log.Warn(ex, "Unable to delete temporary file after S3 upload: " + oldFull);
					}
				}
				else
				{
					throw new InvalidOperationException("Failed to upload file to S3");
				}
			}
			catch (Exception x)
			{
				s_log.Error(x, "Unable to upload media file to S3: " + oldFull);
				throw;
			}
		}

		foreach (var pv in diff.Removed.Where(m => m.Service == PVService.LocalFile))
		{
			var s3Key = GetS3Key(pv.PVId);
			var thumbKey = GetS3ThumbKey(pv.PVId);
			
			// Delete main file
			if (!await DeleteFileFromS3Async(s3Key))
			{
				s_log.Warn("Failed to delete media file from S3: " + s3Key);
			}
			
			// Delete thumbnail if it exists
			if (!await DeleteFileFromS3Async(thumbKey))
			{
				s_log.Warn("Failed to delete thumbnail from S3: " + thumbKey);
			}
		}
	}

	// Synchronous wrapper for backward compatibility
	public void SyncLocalFilePVs(CollectionDiff<PVForSong, PVForSong> diff, int songId)
	{
		SyncS3FilePVsAsync(diff, songId).GetAwaiter().GetResult();
	}
}