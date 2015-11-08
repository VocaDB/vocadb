using System;
using System.Collections.Generic;
using System.Linq;
using NHibernate;
using NHibernate.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain.Tags;

namespace VocaDb.Model.Service.Helpers {

	public static class TagHelpers {

		public static Dictionary<string, Tag> GetTags(ISession session, string[] tagNames) {

			if (tagNames.Length < 20) {
				var direct = session.Query<Tag>().Where(t => tagNames.Contains(t.Name)).ToArray();
				return direct.Union(direct.Where(t => t.AliasedTo != null).Select(t => t.AliasedTo)).ToDictionary(t => t.Name, StringComparer.InvariantCultureIgnoreCase);
			} else {
				return session.Query<Tag>().ToDictionary(t => t.Name);
			}

		}

		public static Dictionary<string, Tag> GetTags(IDatabaseContext<Tag> session, string[] tagNames) {

			if (tagNames.Length < 20) {
				var direct = session.Query().Where(t => tagNames.Contains(t.Name)).ToArray();
				return direct.Union(direct.Where(t => t.AliasedTo != null).Select(t => t.AliasedTo)).ToDictionary(t => t.Name, StringComparer.InvariantCultureIgnoreCase);
			} else {
				return session.Query().ToDictionary(t => t.Name, StringComparer.InvariantCultureIgnoreCase);
			}

		}

	}
}
