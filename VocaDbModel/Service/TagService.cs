using System.Linq;
using NHibernate;
using NHibernate.Linq;
using NLog;
using VocaDb.Model.Domain.Activityfeed;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service {

	public class TagService : ServiceBase {

		private static readonly Logger log = LogManager.GetCurrentClassLogger();

		private Tag GetTag(ISession session, string name) {

			try {

				var tag = session.Load<Tag>(name);

				if (name != tag.TagName)
					tag = session.Load<Tag>(tag.TagName);

				return tag;

			} catch (ObjectNotFoundException) {
				log.Warn("Tag not found: {0}", name);
				return null;
			}

		}

		public TagService(ISessionFactory sessionFactory, IUserPermissionContext permissionContext, IEntryLinkFactory entryLinkFactory) 
			: base(sessionFactory, permissionContext, entryLinkFactory) {}

		public void Archive(ISession session, Tag tag, TagDiff diff, EntryEditEvent reason) {

			var agentLoginData = SessionHelper.CreateAgentLoginData(session, PermissionContext);
			var archived = tag.CreateArchivedVersion(diff, agentLoginData, reason);
			session.Save(archived);

		}

		/// <summary>
		/// Attempts to find a single tag by name. Partial match is allowed.
		/// </summary>
		/// <param name="tagName">Tag name. Cannot be null or empty.</param>
		/// <returns>First tag that matches the name. Can be null if nothing was found.</returns>
		public TagContract FindTag(string tagName) {

			ParamIs.NotNullOrEmpty(() => tagName);

			return HandleQuery(session => {

				Tag[] tags;

				if (tagName.Length < 3)
					tags = session.Query<Tag>().Where(t => t.Name == tagName).Take(1).ToArray();
				else
					tags = session.Query<Tag>().Where(t => t.Name.Contains(tagName)).Take(10).ToArray();

				var match = tags.FirstOrDefault(t => t.Name.Equals(tagName, System.StringComparison.InvariantCultureIgnoreCase));

				if (match == null)
					match = tags.FirstOrDefault();
				
				if (match == null)
					return null;

				return new TagContract(match);

			});

		}

		/// <summary>
		/// Attempts to get a tag by exact name.
		/// </summary>
		/// <param name="tagName">Tag name to be matched. Can be null or empty, in which case null is returned.</param>
		/// <returns>The matched tag, if any. Can be null if nothing was found.</returns>
		public TagContract GetTag(string tagName) {

			if (string.IsNullOrEmpty(tagName))
				return null;

			return HandleQuery(session => {

				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;

				return new TagContract(tag);

			});

		}

		public string[] GetTagNames() {

			return HandleQuery(session => session.Query<Tag>().OrderBy(t => t.Name).Select(t => t.Name).ToArray());

		}

		public TagWithArchivedVersionsContract GetTagWithArchivedVersions(string tagName) {

			return HandleQuery(session => {

				var tag = GetTag(session, tagName);

				if (tag == null)
					return null;

				return new TagWithArchivedVersionsContract(tag);

			});

		}

		public TagCategoryContract[] GetTagsByCategories() {

			return HandleQuery(session => {

				var tags = session.Query<Tag>()
					.Where(t => t.AliasedTo == null)
					.OrderBy(t => t.Name)
					.ToArray()					
					.GroupBy(t => t.CategoryName)
					.ToArray();

				var empty = tags.Where(c => c.Key == string.Empty);

				var tagsByCategories = tags
					.Except(empty).Concat(empty)
					.Select(t => new TagCategoryContract(t.Key, t)).ToArray();

				return tagsByCategories;

			});

		}

	}

}
