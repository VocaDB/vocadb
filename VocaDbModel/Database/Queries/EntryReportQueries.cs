#nullable disable

using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;

namespace VocaDb.Model.Database.Queries
{
	public class EntryReportQueries : QueriesBase<IEntryReportRepository, EntryReport>
	{
		public EntryReportQueries(IEntryReportRepository repository, IUserPermissionContext permissionContext)
			: base(repository, permissionContext)
		{
		}

		public int GetNewReportsCount()
		{
			PermissionContext.VerifyPermission(PermissionToken.ManageEntryReports);

			return HandleQuery(ctx =>
			{
				var cutoff = DateTime.Now - TimeSpan.FromDays(7);
				var count = ctx.Query().Count(r => r.Status == ReportStatus.Open && r.Created >= cutoff);
				return count;
			});
		}
	}
}