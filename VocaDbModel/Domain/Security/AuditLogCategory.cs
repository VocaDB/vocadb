namespace VocaDb.Model.Domain.Security {

	public enum AuditLogCategory {
	
		Unspecified,

		/// <summary>
		/// Admin tasks
		/// </summary>
		Admin,

		/// <summary>
		/// Entry was commented
		/// </summary>
		EntryComment,

		/// <summary>
		/// Entry was created
		/// </summary>
		EntryCreate,

		/// <summary>
		/// Entry was rated
		/// </summary>
		EntryRate,

		/// <summary>
		/// Entry was reported
		/// </summary>
		EntryReport,

		/// <summary>
		/// Entry was tagged
		/// </summary>
		EntryTag,

		/// <summary>
		/// Entry was updated
		/// </summary>
		EntryUpdate,

		/// <summary>
		/// New user was created successfully
		/// </summary>
		UserCreateSuccess,

		/// <summary>
		/// New user failed CAPTCHA
		/// </summary>
		UserCreateFailCaptcha,

		/// <summary>
		/// User logged in successfully
		/// </summary>
		UserLoginSuccess,

		/// <summary>
		/// User failed to log in (invalid password or user not found)
		/// </summary>
		UserLoginFail,

		/// <summary>
		/// User was updated
		/// </summary>
		UserUpdate

	}

}