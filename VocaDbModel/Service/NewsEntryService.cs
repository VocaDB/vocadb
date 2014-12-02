using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.DataContracts.Activityfeed;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Security;

namespace VocaDb.Model.Service {

	public class NewsEntryService : ServiceBase {

		public NewsEntryService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public void Delete(int id) {

			DeleteEntity<NewsEntry>(id, PermissionToken.DeleteEntries);

		}

		public NewsEntryContract GetNewsEntry(int id) {

			return HandleQuery(session => new NewsEntryContract(session.Load<NewsEntry>(id)));

		}

		public NewsEntryContract[] GetNewsEntries(int maxEntries) {

			return HandleQuery(session => {

				var entries = session.Query<NewsEntry>().OrderByDescending(a => a.CreateDate).Take(maxEntries).ToArray();

				var contracts = entries.Select(e => new NewsEntryContract(e)).ToArray();

				return contracts;

			});

		}

		public void UpdateNewsEntry(NewsEntryContract contract) {

			ParamIs.NotNull(() => contract);

			PermissionContext.VerifyPermission(PermissionToken.EditNews);

			HandleTransaction(session => {

				var user = GetLoggedUser(session);

				if (contract.Id == 0) {

					var entry = new NewsEntry(contract.Text, user, contract.Anonymous, contract.Important, contract.Stickied);
					session.Save(entry);

					AuditLog("created " + entry, session, user);

				} else {

					var entry = session.Load<NewsEntry>(contract.Id);
					entry.Anonymous = contract.Anonymous;
					entry.Important = contract.Important;
					entry.Stickied = contract.Stickied;
					entry.Text = contract.Text;

					session.Update(entry);

					AuditLog("updated " + entry, session, user);

				}

			});

		}

	}
 
}
