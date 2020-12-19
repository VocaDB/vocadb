#nullable disable

using System.Net;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using NLog;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Model.Helpers
{
	public class ReCaptcha2
	{
		public const string ResponseFieldName = "g-recaptcha-response";
		private static readonly ILogger _log = LogManager.GetCurrentClassLogger();
		private const string VerifyApi = "https://www.google.com/recaptcha/api/siteverify";

		public static async Task<ValidateCaptchaResponse> ValidateAsync(IHttpRequest request, string privateKey)
		{
			var userResponse = request.Form[ResponseFieldName];

			if (string.IsNullOrEmpty(userResponse))
			{
				_log.Warn("CAPTCHA response was empty");
				return new ValidateCaptchaResponse(false);
			}

			var userIp = request.UserHostAddress;

			var requestUrl = $"{VerifyApi}?secret={privateKey}&response={userResponse}&remoteip={userIp}";
			VerifyResponse verifyResponse;

			try
			{
				verifyResponse = await JsonRequest.ReadObjectAsync<VerifyResponse>(requestUrl);
			}
			catch (WebException x)
			{
				_log.Error(x, "Unable to get response from ReCAPTCHA");
				return new ValidateCaptchaResponse(false);
			}

			return new ValidateCaptchaResponse(verifyResponse.Success,
				userResponse,
				verifyResponse.ErrorCodes != null ? string.Join(", ", verifyResponse.ErrorCodes) : string.Empty);
		}

		[DataContract]
		public class VerifyResponse
		{
			[DataMember(Name = "error-codes")]
			public string[] ErrorCodes { get; set; }

			[DataMember]
			public bool Success { get; set; }
		}
	}

	public class ValidateCaptchaResponse
	{
		public ValidateCaptchaResponse(bool success, string userResponse = "", string errorCodes = "")
		{
			Error = errorCodes;
			UserResponse = userResponse;
			Success = success;
		}

		public string Error { get; private set; }

		public bool Success { get; private set; }

		public string UserResponse { get; set; }
	}
}
