#nullable disable

using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Database.Repositories
{
	public interface IAuditLogger
	{
		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// This override uses the currently logged in user, if any.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		void SysLog(string doingWhat);

		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		/// <param name="who">Who made the action.</param>
		void SysLog(string doingWhat, string who);

		void AuditLog(string doingWhat, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified);

		void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified);

		void AuditLog(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null);

		Task AuditLogAsync(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null);
	}
}