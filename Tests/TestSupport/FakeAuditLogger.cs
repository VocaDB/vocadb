using System;
using System.Threading.Tasks;
using VocaDb.Model;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

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

		public void AuditLog(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null) {
			SysLog(doingWhat, user != null ? user.Name : "Unknown");
		}

		public void SysLog(string doingWhat) {
			SysLog(doingWhat, "Unknown");
		}

		public void SysLog(string doingWhat, string who) {
			Console.WriteLine(GetAuditLogMessage(doingWhat, who));
		}

		public Task AuditLogAsync(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null) {
			AuditLog(doingWhat, user, category, entryId);
			return Task.CompletedTask;
		}

	}
}
