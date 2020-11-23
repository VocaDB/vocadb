using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Globalization;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Domain.Web;

namespace VocaDb.Model.Domain.Security
{

	public class UserSettingCulture : UserSetting<string>
	{

		public UserSettingCulture(IHttpContext context, IUserPermissionContext permissionContext) : base(context, permissionContext)
		{
		}

		protected override string RequestParamName => "culture";
		protected override string SettingName => "Culture";
		protected override string GetPersistedValue(UserWithPermissionsContract permissionContext) => permissionContext.Culture;

		protected override void SetPersistedValue(User user, string val)
		{
			user.Culture = val;
		}

		protected override void SetPersistedValue(UserWithPermissionsContract user, string val)
		{
			user.Culture = val;
		}

		protected override bool TryParseValue(string str, out string val)
		{
			val = str;
			return true;
		}
	}

	public class UserSettingLanguage : UserSetting<string>
	{

		public UserSettingLanguage(IHttpContext context, IUserPermissionContext permissionContext) : base(context, permissionContext)
		{
		}

		protected override string RequestParamName => "culture";
		protected override string SettingName => "Language";
		protected override string GetPersistedValue(UserWithPermissionsContract permissionContext) => permissionContext.Language;

		protected override void SetPersistedValue(User user, string val)
		{
			user.Language = new OptionalCultureCode(val);
		}

		protected override void SetPersistedValue(UserWithPermissionsContract user, string val)
		{
			user.Language = val;
		}

		protected override bool TryParseValue(string str, out string val)
		{
			val = str;
			return true;
		}
	}

}
