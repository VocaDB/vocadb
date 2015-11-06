using System;
using System.Linq;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Service;
using VocaDb.Model.Service.Repositories;

namespace VocaDb.Model.Queries {

	public class EntryReportQueries : QueriesBase<IEntryReportRepository, EntryReport> {

		public EntryReportQueries(IEntryReportRepository repository, IUserPermissionContext permissionContext)
			: base(repository, permissionContext) {
		}

		public int GetNewReportsCount() {
			
			PermissionContext.VerifyPermission(PermissionToken.ManageEntryReports);

			return HandleQuery(ctx => {

				var cutoff = DateTime.Now - TimeSpan.FromDays(7);
				var count = ctx.Query().Count(r => r.Created >= cutoff);
				return count;

			});

		}

	}

}