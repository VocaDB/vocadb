using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Utils;
using VocaDb.Web.Code;

namespace VocaDb.Web.Models.Shared
{
	public sealed record GlobalValues
	{
		public string? LockdownMessage { get; init; }

		public string? BaseAddress { get; init; }
		[JsonConverter(typeof(StringEnumConverter))]
		public ContentLanguagePreference LanguagePreference { get; init; }
		public bool IsLoggedIn { get; init; }
		public int LoggedUserId { get; init; }
		public SanitizedUserWithPermissionsContract? LoggedUser { get; init; }
		public string Culture { get; init; }
		public string UICulture { get; init; }

		public GlobalValues(VocaDbPage model)
		{
			LockdownMessage = AppConfig.LockdownMessage;

			BaseAddress = model.RootPath;
			LanguagePreference = model.UserContext.LanguagePreference;
			IsLoggedIn = model.UserContext.IsLoggedIn;
			LoggedUserId = model.UserContext.LoggedUserId;
			LoggedUser = model.UserContext.LoggedUser is ServerOnlyUserWithPermissionsContract loggedUser ? new SanitizedUserWithPermissionsContract(loggedUser) : null;
			Culture = model.Culture;
			UICulture = model.UICulture;
		}
	}
}
