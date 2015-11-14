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

		private bool IsValid(TagBaseContract contract) {

			if (contract == null)
				return false;

			if (contract.Id != 0)
				return true;

			return !string.IsNullOrEmpty(contract.Name) && Tag.IsValidTagName(contract.Name);

		}

		public TagUsageForApiContract[] AddTags<TEntry, TTag>(int entryId, string[] tags,
			bool onlyAdd,
			IRepository<User> repository, IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Func<TEntry, TagManager<TTag>> tagFunc,
			Func<TEntry, IDatabaseContext<TTag>, ITagUsageFactory<TTag>> tagUsageFactoryFactory)
			where TEntry : IEntryBase where TTag : TagUsage {

			return AddTags(entryId, tags.Select(t => new TagBaseContract { Name = t }).ToArray(), onlyAdd, repository,
				permissionContext, entryLinkFactory, tagFunc, tagUsageFactoryFactory);

		}

		public TagUsageForApiContract[] AddTags<TEntry, TTag>(int entryId, TagBaseContract[] tags, 
			bool onlyAdd,
			IRepository<User> repository, IUserPermissionContext permissionContext,
			IEntryLinkFactory entryLinkFactory,
			Func<TEntry, TagManager<TTag>> tagFunc,
			Func<TEntry, IDatabaseContext<TTag>, ITagUsageFactory<TTag>> tagUsageFactoryFactory) 
			where TEntry : IEntryBase where TTag : TagUsage {
			
			ParamIs.NotNull(() => tags);

			permissionContext.VerifyManageDatabase();

			tags = tags.Where(IsValid).ToArray();

			if (onlyAdd && !tags.Any())
				return new TagUsageForApiContract[0];

			return repository.HandleTransaction(ctx => {
				
				var tagIdsWithoutName = tags.Where(t => string.IsNullOrEmpty(t.Name)).Select(t => t.Id).ToArray();
				var tagsWithName = tags.Select(t => t.Name).Where(t => !string.IsNullOrEmpty(t));
				var tagFromIds = ctx.Query<Tag>().Where(t => tagIdsWithoutName.Contains(t.Id)).Select(t => t.Name).ToArray();
				var tagNames = tagsWithName.Concat(tagFromIds).Distinct(StringComparer.InvariantCultureIgnoreCase).ToArray();

				var user = ctx.OfType<User>().GetLoggedUser(permissionContext);
				var entry = ctx.OfType<TEntry>().Load(entryId);

				ctx.AuditLogger.AuditLog(string.Format("tagging {0} with {1}",
					entryLinkFactory.CreateEntryLink(entry), string.Join(", ", tagNames)), user);

				var tagFactory = new TagFactoryRepository(ctx.OfType<Tag>(), new AgentLoginData(user));
				var tagUsageFactory = tagUsageFactoryFactory(entry, ctx.OfType<TTag>());
				var existingTags = TagHelpers.GetTags(ctx.OfType<Tag>(), tagNames);

				tagFunc(entry).SyncVotes(user, tagNames, existingTags, tagFactory, tagUsageFactory, onlyAdd: onlyAdd);

				return tagFunc(entry).Usages.Select(t => new TagUsageForApiContract(t)).ToArray();

			});

		}

	}

}
