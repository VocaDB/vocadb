using System;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Queries {

	public class TagUsageQueries {

		public TagUsageForApiContract[] AddTags<TEntry, TTag>(int entryId, string[] tags, 
			bool onlyAdd,
			IRepository<User> repository, IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Func<TEntry, TagManager<TTag>> tagFunc,
			Func<TEntry, IDatabaseContext<TTag>, ITagUsageFactory<TTag>> tagUsageFactoryFactory) 
			where TEntry : IEntryBase where TTag : TagUsage {
			
			ParamIs.NotNull(() => tags);

			permissionContext.VerifyManageDatabase();

			tags = tags.Where(t => !string.IsNullOrEmpty(t) && Tag.IsValidTagName(t)).ToArray();

			if (onlyAdd && !tags.Any())
				return new TagUsageForApiContract[0];

			return repository.HandleTransaction(ctx => {
				
				tags = tags.Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

				var user = ctx.OfType<User>().GetLoggedUser(permissionContext);
				var entry = ctx.OfType<TEntry>().Load(entryId);

				ctx.AuditLogger.AuditLog(string.Format("tagging {0} with {1}",
					entryLinkFactory.CreateEntryLink(entry), string.Join(", ", tags)), user);

				var tagFactory = new TagFactoryRepository(ctx.OfType<Tag>(), new AgentLoginData(user));
				var tagUsageFactory = tagUsageFactoryFactory(entry, ctx.OfType<TTag>());
				var existingTags = TagHelpers.GetTags(ctx.OfType<Tag>(), tags);

				tagFunc(entry).SyncVotes(user, tags, existingTags, tagFactory, tagUsageFactory, onlyAdd: onlyAdd);

				return tagFunc(entry).Usages.Select(t => new TagUsageForApiContract(t)).ToArray();

			});

		}

	}

}
