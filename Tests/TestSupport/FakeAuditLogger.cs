using System;
using VocaDb.Model;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Tests.TestSupport {

	public class FakeAuditLogger : IAuditLogger {

		private string GetAuditLogMessage(string doingWhat, string who) {

			return string.Format("'{0}' {1}", who, doingWhat);

		}

		public void AuditLog(string doingWhat, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified) {
			ParamIs.NotNull(() => who);
			SysLog(doingWhat, who.Name);
		}

		public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified) {
			SysLog(doingWhat, who);
		}

		public void AuditLog(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified) {
			SysLog(doingWhat, user != null ? user.Name : "Unknown");
		}

		public void SysLog(string doingWhat) {
			SysLog(doingWhat, "Unknown");
		}

		public void SysLog(string doingWhat, string who) {
			Console.WriteLine(GetAuditLogMessage(doingWhat, who));
		}

	}
}
