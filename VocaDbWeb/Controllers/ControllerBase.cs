#nullable disable

using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using NLog;
using VocaDb.Model;
using VocaDb.Model.DataContracts;
using VocaDb.Model.DataContracts.UseCases;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Images;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Helpers;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;
using VocaDb.Web.Helpers;

namespace VocaDb.Web.Controllers
{
	// TODO: implement
	public class ControllerBase : Controller
	{
		private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
		protected static readonly TimeSpan ImageExpirationTime = TimeSpan.FromMinutes(5);
		protected const int EntriesPerPage = 30;
		protected const int InvalidId = 0;
		protected static readonly TimeSpan PictureCacheDuration = TimeSpan.FromDays(30);
		protected const int PictureCacheDurationSec = 30 * 24 * 60 * 60;
		protected const int StatsCacheDurationSec = 24 * 60 * 60;

		protected ControllerBase()
		{
			PageProperties.OpenGraph.Image = VocaUriBuilder.StaticResource("/img/vocaDB-title-large.png");
		}

		protected string Hostname => WebHelper.GetRealHost(Request);

		protected int LoggedUserId
		{
			get
			{
				PermissionContext.VerifyLogin();

				return PermissionContext.LoggedUser.Id;
			}
		}

		protected PagePropertiesData PageProperties => PagePropertiesData.Get(ViewBag);

		protected IUserPermissionContext PermissionContext => HttpContext.RequestServices.GetRequiredService<IUserPermissionContext>();

		protected Login Login => HttpContext.RequestServices.GetRequiredService<Login>();

#nullable enable
		protected string GetHostnameForValidHit() => WebHelper.GetHostnameForValidHit(Request);

		protected ActionResult NoId()
		{
			return NotFound("No ID specified");
		}

		public static void AddFormSubmissionError(Microsoft.AspNetCore.Mvc.ControllerBase controller, string details)
		{
			s_log.Warn("Form submission error: {0}", details);
			controller.ModelState.AddModelError(string.Empty, $"Error while sending form contents - please try again. Diagnostic error message: {details}.");
		}

		protected void AddFormSubmissionError(string details)
		{
			AddFormSubmissionError(this, details);
		}
#nullable disable

		protected ActionResult Picture(EntryForPictureDisplayContract contract)
		{
			ParamIs.NotNull(() => contract);

			var cacheControl = new CacheControlHeaderValue
			{
				// Allow images to be cached by public proxies, images shouldn't contain anything sensitive so this should be ok.
				Public = true,
			};

			Response.Headers[HeaderNames.ETag] = $"{contract.EntryType}{contract.EntryId}v{contract.Version}";

			// Cached version indicated by the "v" request parameter.
			// If no version is specified, assume no caching.
			if (contract.Version > 0 && !string.IsNullOrEmpty(Request.Query["v"]))
				cacheControl.MaxAge = PictureCacheDuration;

			Response.GetTypedHeaders().CacheControl = cacheControl;

			return Picture(contract.Picture, contract.Name);
		}

		protected void CheckConcurrentEdit(EntryType entryType, int id)
		{
			Login.Manager.VerifyLogin();

			var conflictingEditor = ConcurrentEntryEditManager.CheckConcurrentEdits(new EntryRef(entryType, id), Login.User);

			if (conflictingEditor.UserId != ConcurrentEntryEditManager.Nothing.UserId)
			{
				var ago = DateTime.Now - conflictingEditor.Time;

				if (ago.TotalMinutes < 1)
				{
					TempData.SetStatusMessage(string.Format(ViewRes.EntryEditStrings.ConcurrentEditWarningNow, conflictingEditor.UserName));
				}
				else
				{
					TempData.SetStatusMessage(string.Format(ViewRes.EntryEditStrings.ConcurrentEditWarning, conflictingEditor.UserName, (int)ago.TotalMinutes));
				}
			}
		}

#nullable enable
		public static bool CheckUploadedPicture(Microsoft.AspNetCore.Mvc.ControllerBase controller, IFormFile pictureUpload, string fieldName)
		{
			bool errors = false;

			if (pictureUpload.Length > ImageHelper.MaxImageSizeBytes)
			{
				controller.ModelState.AddModelError(fieldName, "Picture file is too large.");
				errors = true;
			}

			if (!ImageHelper.IsValidImageExtension(pictureUpload.FileName))
			{
				controller.ModelState.AddModelError(fieldName, "Picture format is not valid.");
				errors = true;
			}

			return !errors;
		}

		protected bool CheckUploadedPicture(IFormFile pictureUpload, string fieldName)
		{
			return CheckUploadedPicture(this, pictureUpload, fieldName);
		}
#nullable disable

		protected ActionResult HttpStatusCodeResult(HttpStatusCode code, string message)
		{
			Response.StatusCode = (int)code;
			// TODO: implement Response.StatusDescription = message;

			return Content((int)code + ": " + message);
		}

#nullable enable
		public static void ParseAdditionalPictures(Microsoft.AspNetCore.Mvc.ControllerBase controller, IFormFile? mainPic, IList<EntryPictureFileContract> pictures)
		{
			ParamIs.NotNull(() => pictures);

			var additionalPics = Enumerable.Range(0, controller.Request.Form.Files.Count)
				.Select(i => controller.Request.Form.Files[i])
				.Where(f => f is not null && f.FileName != mainPic?.FileName)
				.ToArray();

			var newPics = pictures.Where(p => p.Id == 0).ToArray();

			for (int i = 0; i < additionalPics.Length; ++i)
			{
				if (i >= newPics.Length)
					break;

				var contract = ParsePicture(controller, additionalPics[i], "Pictures", ImagePurpose.Additional);

				if (contract is not null)
				{
					newPics[i].OriginalFileName = contract.OriginalFileName;
					newPics[i].UploadedFile = contract.UploadedFile;
					newPics[i].Mime = contract.Mime;
					newPics[i].ContentLength = contract.ContentLength;
					newPics[i].Purpose = contract.Purpose;
				}
			}

			CollectionHelper.RemoveAll(pictures, p => p.Id == 0 && p.UploadedFile is null);
		}

		protected void ParseAdditionalPictures(IFormFile? mainPic, IList<EntryPictureFileContract> pictures)
		{
			ParseAdditionalPictures(this, mainPic, pictures);
		}

		public static EntryPictureFileContract? ParsePicture(Microsoft.AspNetCore.Mvc.ControllerBase controller, IFormFile? pictureUpload, string fieldName, ImagePurpose purpose)
		{
			EntryPictureFileContract? pictureData = null;

			if (controller.Request.Form.Files.Count > 0 && pictureUpload is not null && pictureUpload.Length > 0)
			{
				if (pictureUpload.Length > ImageHelper.MaxImageSizeBytes)
				{
					controller.ModelState.AddModelError(fieldName, "Picture file is too large.");
					return null;
				}

				if (!ImageHelper.IsValidImageExtension(pictureUpload.FileName))
				{
					controller.ModelState.AddModelError(fieldName, "Picture format is not valid.");
					return null;
				}

				pictureData = new(pictureUpload.OpenReadStream(), pictureUpload.ContentType, (int)pictureUpload.Length, purpose)
				{
					OriginalFileName = pictureUpload.FileName
				};
			}

			return pictureData;
		}

		protected EntryPictureFileContract? ParsePicture(IFormFile? pictureUpload, string fieldName, ImagePurpose purpose)
		{
			return ParsePicture(this, pictureUpload, fieldName, purpose);
		}
#nullable disable

		protected ActionResult Picture(PictureContract pictureData, string title)
		{
			if (pictureData?.Bytes == null || string.IsNullOrEmpty(pictureData.Mime))
				return File("~/Content/unknown.png", "image/png");

			var ext = ImageHelper.GetExtensionFromMime(pictureData.Mime);

			if (!string.IsNullOrEmpty(ext))
			{
				//var encoded = Url.Encode(title);
				// Note: there is no good way to encode content-disposition filename (see http://stackoverflow.com/a/216777)
				Response.Headers.Add("content-disposition", $"inline;filename=\"{title}{ext}\"");
			}

			return File(pictureData.Bytes, pictureData.Mime);
		}

		protected ActionResult LowercaseJson(object obj) => Content(JsonHelpers.Serialize(obj), "application/json");

		protected new ActionResult Json(object obj)
		{
			return Content(JsonConvert.SerializeObject(obj), "application/json");
		}

		protected ActionResult Json(object obj, string jsonPCallback)
		{
			if (string.IsNullOrEmpty(jsonPCallback))
				return Json(obj);

			return Content($"{jsonPCallback}({JsonConvert.SerializeObject(obj)})", "application/json");
		}

		protected Task<string> RenderPartialViewToStringAsync(string viewName, object model) => HttpContext.RequestServices.GetRequiredService<IViewRenderService>().RenderToStringAsync(viewName, model);

		protected void RestoreErrorsFromTempData()
		{
			// TODO: implement
		}

		protected void SaveErrorsToTempData()
		{
			// TODO: implement
		}

		protected void SetSearchEntryType(EntryType entryType)
		{
			PageProperties.GlobalSearchType = entryType;
		}

		protected VocaUrlMapper UrlMapper => new VocaUrlMapper();

		protected ActionResult Xml(string content)
		{
			if (string.IsNullOrEmpty(content))
				return new EmptyResult();

			return Content(content, "text/xml", Encoding.UTF8);
		}
	}

	public enum DataFormat
	{
		Auto,

		Json,

		Xml
	}
}