using System.Net;
using System.Runtime.Serialization;
using NLog;

namespace VocaDb.Model.Helpers;

public class ReCaptcha2
{
	public const string ResponseFieldName = "g-recaptcha-response";
	private static readonly ILogger s_log = LogManager.GetCurrentClassLogger();
	private const string VerifyApi = "https://www.google.com/recaptcha/api/siteverify";

	public static async Task<ValidateCaptchaResponse> ValidateAsync(string userResponse, string userIp, string privateKey)
	{
		if (string.IsNullOrEmpty(userResponse))
		{
			s_log.Warn("CAPTCHA response was empty");
			return new ValidateCaptchaResponse(success: false);
		}

		try
		{
			var requestUrl = $"{VerifyApi}?secret={privateKey}&response={userResponse}&remoteip={userIp}";
			var verifyResponse = (await JsonRequest.ReadObjectAsync<VerifyResponse>(requestUrl))!;

			return new ValidateCaptchaResponse(
				success: verifyResponse.Success,
				userResponse: userResponse,
				errorCodes: verifyResponse.ErrorCodes != null
					? string.Join(", ", verifyResponse.ErrorCodes)
					: string.Empty
			);
		}
		catch (WebException x)
		{
			s_log.Error(x, "Unable to get response from ReCAPTCHA");
			return new ValidateCaptchaResponse(success: false);
		}
	}

	[DataContract]
	public class VerifyResponse
	{
		[DataMember(Name = "error-codes")]
		public string[]? ErrorCodes { get; set; }

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
