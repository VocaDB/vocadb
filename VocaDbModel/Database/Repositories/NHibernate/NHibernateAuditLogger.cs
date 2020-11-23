using NLog;
using System.Threading.Tasks;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Users;

namespace VocaDb.Model.Database.Repositories.NHibernate
{

	public class NHibernateAuditLogger : IAuditLogger
	{

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private IDatabaseContext<AuditLogEntry> Ctx { get; set; }
		private IUserPermissionContext PermissionContext { get; set; }

		public NHibernateAuditLogger(IDatabaseContext<AuditLogEntry> ctx, IUserPermissionContext permissionContext)
		{
			Ctx = ctx;
			PermissionContext = permissionContext;
		}

		private static AgentLoginData CreateAgentLoginData(IDatabaseContext<AuditLogEntry> ctx, IUserPermissionContext permissionContext, User user = null)
		{

			if (user != null)
				return new AgentLoginData(user);

			if (permissionContext.LoggedUser != null)
			{

				user = ctx.OfType<User>().Load(permissionContext.LoggedUser.Id);
				return new AgentLoginData(user);

			}
			else
			{

				return new AgentLoginData(permissionContext.Name);

			}

		}

		private string GetAuditLogMessage(string doingWhat, string who)
		{

			return string.Format("'{0}' {1}", who, doingWhat);

		}

		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// This override uses the currently logged in user, if any.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		public void SysLog(string doingWhat)
		{

			SysLog(doingWhat, PermissionContext.Name);

		}

		/// <summary>
		/// Logs an action in syslog. 
		/// Syslog is saved through NLog to a file.
		/// </summary>
		/// <param name="doingWhat">What the user was doing.</param>
		/// <param name="who">Who made the action.</param>
		public void SysLog(string doingWhat, string who)
		{

			log.Info(GetAuditLogMessage(doingWhat, who));

		}

		public void AuditLog(string doingWhat, AgentLoginData who, AuditLogCategory category = AuditLogCategory.Unspecified)
		{

			ParamIs.NotNull(() => who);

			SysLog(doingWhat, who.Name);

			var entry = new AuditLogEntry(who, doingWhat, category, GlobalEntryId.Empty);

			Ctx.Save(entry);

		}

		public void AuditLog(string doingWhat, string who, AuditLogCategory category = AuditLogCategory.Unspecified)
		{

			SysLog(doingWhat, who);

			var agentLoginData = new AgentLoginData(who);
			var entry = new AuditLogEntry(agentLoginData, doingWhat, category, GlobalEntryId.Empty);

			Ctx.Save(entry);

		}

		public void AuditLog(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null)
		{

			var agentLoginData = CreateAgentLoginData(Ctx, PermissionContext, user);
			SysLog(doingWhat, agentLoginData.Name);
			var entry = new AuditLogEntry(agentLoginData, doingWhat, category, entryId ?? GlobalEntryId.Empty);

			Ctx.Save(entry);

		}

		public async Task AuditLogAsync(string doingWhat, User user = null, AuditLogCategory category = AuditLogCategory.Unspecified, GlobalEntryId? entryId = null)
		{

			var agentLoginData = CreateAgentLoginData(Ctx, PermissionContext, user);
			SysLog(doingWhat, agentLoginData.Name);
			var entry = new AuditLogEntry(agentLoginData, doingWhat, category, entryId ?? GlobalEntryId.Empty);

			await Ctx.SaveAsync(entry);

		}

	}
}
