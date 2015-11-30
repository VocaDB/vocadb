using System;
using System.Collections.Generic;
using System.Linq;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.Helpers;

namespace VocaDb.Model.Service.Queries {

	public class TagUsageQueries {

		/// <summary>
		/// Gets the next free name for a tag
		/// </summary>
		private string GetNewTagName(IDatabaseContext<User> ctx, string tagName) {

			var prefixMatch = ctx.Query<Tag>().Where(t => t.Name.StartsWith(tagName)).Select(t => t.Name).ToArray();

			if (!prefixMatch.Any())
				return tagName;

			var hash = new HashSet<string>(prefixMatch, StringComparer.InvariantCultureIgnoreCase);
			return Enumerable.Range(0, int.MaxValue).Select(i => string.Format("{0}{1}", tagName, i)).First(name => !hash.Contains(name));

		}

		private bool IsValid(TagBaseContract contract) {

			if (contract == null)
				return false;

			if (HasId(contract))
				return true;

			return !string.IsNullOrEmpty(contract.Name) && Tag.IsValidTagName(contract.Name);

		}

		private bool HasId(TagBaseContract contract) {
			return contract.Id > 0;
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
				
				// Tags are primarily added by Id, secondarily by translated name.
				// First separate given tags for tag IDs and tag names
				var tagIds = tags.Where(HasId).Select(t => t.Id).ToArray();
				var translatedTagNames = tags.Where(t => !HasId(t) && !string.IsNullOrEmpty(t.Name)).Select(t => t.Name).ToArray();

				// Load existing tags by name and ID.
				var tagsFromNames = ctx.Query<Tag>().Where(t => translatedTagNames.Contains(t.EnglishName))
					.Select(t => new TagNameAndTranslation { TagName = t.Name, EnglishName = t.EnglishName }).ToArray();

				var tagsFromIds = ctx.Query<Tag>().Where(t => tagIds.Contains(t.Id))
					.Select(t => new TagNameAndTranslation { TagName = t.Name, EnglishName = t.EnglishName }).ToArray();

				// Figure out tags that don't exist yet (no ID and no matching name).
				var newTags = translatedTagNames.Except(tagsFromNames.Select(t => t.EnglishName)).Select(t => new TagNameAndTranslation(GetNewTagName(ctx, t), t)).ToArray();

				// Get the final list of tag names with translations
				var appliedTags = tagsFromNames.Concat(tagsFromIds).Concat(newTags).Distinct(t => t.TagName, StringComparer.InvariantCultureIgnoreCase).ToArray();
				var tagNames = appliedTags.Select(t => t.TagName).ToArray();

				var user = ctx.OfType<User>().GetLoggedUser(permissionContext);
				var entry = ctx.OfType<TEntry>().Load(entryId);

				ctx.AuditLogger.AuditLog(string.Format("tagging {0} with {1}",
					entryLinkFactory.CreateEntryLink(entry), string.Join(", ", tagNames)), user);

				var tagFactory = new TagFactoryRepository(ctx.OfType<Tag>(), new AgentLoginData(user));
				var tagUsageFactory = tagUsageFactoryFactory(entry, ctx.OfType<TTag>());
				var existingTags = TagHelpers.GetTags(ctx.OfType<Tag>(), tagNames);

				tagFunc(entry).SyncVotes(user, appliedTags, existingTags, tagFactory, tagUsageFactory, onlyAdd: onlyAdd);

				return tagFunc(entry).Usages.Select(t => new TagUsageForApiContract(t)).ToArray();

			});

		}

	}

}
