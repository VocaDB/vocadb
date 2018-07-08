using System.Web;
using VocaDb.Model.DataContracts.Users;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Domain.Security {

	public class UserSettingCulture : UserSetting<string> {

		public UserSettingCulture(HttpContext context, IUserPermissionContext permissionContext) : base(context, permissionContext) {
		}

		protected override string RequestParamName => "culture";
		protected override string SettingName => "Culture";
		protected override string GetPersistedValue(UserWithPermissionsContract permissionContext) => permissionContext.Culture;

		protected override void SetPersistedValue(User user, string val) {
			user.Culture = val;
		}

		protected override void SetPersistedValue(UserWithPermissionsContract user, string val) {
			user.Culture = val;
		}

		protected override bool TryParseValue(string str, out string val) {
			val = str;
			return true;
		}
	}
}
