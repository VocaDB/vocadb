using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Security;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Service.Helpers;
using VocaDb.Model.Service.QueryableExtenders;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Service.Queries
{

	public class TagUsageQueries
	{

		private static readonly Logger log = LogManager.GetCurrentClassLogger();
		private readonly IUserPermissionContext permissionContext;

		private bool IsValid(TagBaseContract contract)
		{

			if (contract == null)
				return false;

			if (HasId(contract))
				return true;

			return !string.IsNullOrEmpty(contract.Name);

		}

		private bool HasId(TagBaseContract contract)
		{
			return contract.Id > 0;
		}

		public TagUsageQueries(IUserPermissionContext permissionContext)
		{
			this.permissionContext = permissionContext;
		}

		public async Task<TagUsageForApiContract[]> AddTags<TEntry, TTag>(int entryId,
			TagBaseContract[] tags,
			bool onlyAdd,
			IRepository<User> repository,
			IEntryLinkFactory entryLinkFactory,
			IEnumTranslations enumTranslations,
			Func<TEntry, TagManager<TTag>> tagFunc,
			Func<TEntry, IDatabaseContext<TTag>, ITagUsageFactory<TTag>> tagUsageFactoryFactory)
			where TEntry : class, IEntryWithNames, IEntryWithTags
			where TTag : TagUsage
		{

			ParamIs.NotNull(() => tags);

			permissionContext.VerifyPermission(PermissionToken.EditTags);

			tags = tags.Where(IsValid).ToArray();

			if (onlyAdd && !tags.Any())
				return new TagUsageForApiContract[0];

			return await repository.HandleTransactionAsync(async ctx =>
			{

				// Tags are primarily added by Id, secondarily by translated name.
				// First separate given tags for tag IDs and tag names
				var tagIds = tags.Where(HasId).Select(t => t.Id).ToArray();
				var translatedTagNames = tags.Where(t => !HasId(t) && !string.IsNullOrEmpty(t.Name)).Select(t => t.Name).ToArray();

				// Load existing tags by name and ID.
				var tagsFromIds = await ctx.Query<Tag>().Where(t => tagIds.Contains(t.Id)).VdbToListAsync();

				var tagsFromNames = await ctx.Query<Tag>().WhereHasName(translatedTagNames).VdbToListAsync();

				// Figure out tags that don't exist yet (no ID and no matching name).
				var newTagNames = translatedTagNames.Except(tagsFromNames.SelectMany(t => t.Names.AllValues), StringComparer.InvariantCultureIgnoreCase).ToArray();

				var user = await ctx.OfType<User>().GetLoggedUserAsync(permissionContext);
				var tagFactory = new TagFactoryRepository(ctx.OfType<Tag>(), new AgentLoginData(user));
				var newTags = await tagFactory.CreateTagsAsync(newTagNames);

				var entry = await ctx.LoadAsync<TEntry>(entryId);

				// Get the final list of tag names with translations
				var appliedTags = tagsFromNames
					.Concat(tagsFromIds)
					.Where(t => permissionContext.HasPermission(PermissionToken.ApplyAnyTag) || t.IsValidFor(entry.EntryType))
					.Concat(newTags)
					.Distinct()
					.ToArray();

				var tagUsageFactory = tagUsageFactoryFactory(entry, ctx.OfType<TTag>());

				var tagNames = appliedTags.Select(t => t.DefaultName);
				await ctx.AuditLogger.AuditLogAsync(string.Format("tagging {0} with {1}",
					entryLinkFactory.CreateEntryLink(entry), string.Join(", ", tagNames)), user);

				var addedTags = appliedTags.Except(entry.Tags.Tags).ToArray();

				if (entry.AllowNotifications)
				{
					await new FollowedTagNotifier().SendNotificationsAsync(ctx, entry, addedTags, new[] { user.Id }, entryLinkFactory, enumTranslations);
				}

				var updatedTags = tagFunc(entry).SyncVotes(user, appliedTags, tagUsageFactory, onlyAdd: onlyAdd);
				var tagCtx = ctx.OfType<Tag>();

				foreach (var tag in updatedTags)
					await tagCtx.UpdateAsync(tag);

				RecomputeTagUsagesCounts(tagCtx, updatedTags);

				ctx.AuditLogger.SysLog("finished tagging");

				return tagFunc(entry).ActiveUsages.Select(t => new TagUsageForApiContract(t, permissionContext.LanguagePreference)).ToArray();

			});

		}

		public int RemoveTagUsage<TUsage, TEntry>(long tagUsageId, IRepository<TEntry> repository)
			where TUsage : TagUsage
			where TEntry : class, IDatabaseObject
		{

			return repository.HandleTransaction(ctx =>
			{

				ctx.AuditLogger.SysLog(string.Format("deleting tag usage with Id {0}", tagUsageId));

				var tagUsage = ctx.Load<TUsage>(tagUsageId);

				EntryPermissionManager.VerifyAccess(permissionContext, tagUsage.EntryBase, EntryPermissionManager.CanRemoveTagUsages);

				tagUsage.Delete();
				ctx.Delete(tagUsage);
				ctx.Update(tagUsage.Tag);

				ctx.AuditLogger.AuditLog(string.Format("removed {0}", tagUsage));
				ctx.AuditLogger.SysLog("Usage count for " + tagUsage.Tag + " is now " + tagUsage.Tag.UsageCount);

				return tagUsage.EntryBase.Id;

			});

		}

		private Dictionary<int, int> GetActualTagUsageCounts(IDatabaseContext<Tag> ctx, int[] tagIds)
		{

			// Note: these counts are not up to date in tests, only when querying the real DB
			var result = ctx.Query().Where(t => tagIds.Contains(t.Id)).Select(t => new
			{
				Id = t.Id,
				Count = t.AllAlbumTagUsages.Count + t.AllArtistTagUsages.Count
					+ t.AllSongTagUsages.Count + t.AllEventTagUsages.Count
					+ t.AllEventSeriesTagUsages.Count + t.AllSongListTagUsages.Count
			}).ToDictionary(t => t.Id, t => t.Count);

			return result;

		}

		private void RecomputeTagUsagesCounts(IDatabaseContext<Tag> ctx, Tag[] tags)
		{

			var ids = tags.Select(t => t.Id).ToArray();
			var usages = GetActualTagUsageCounts(ctx, ids);

			foreach (var tag in tags)
			{
				if (usages.ContainsKey(tag.Id) && tag.UsageCount != usages[tag.Id])
				{
					// This is expected for tests
					// TODO: come up with a proper fix that works for tests and the real DB
					log.Warn("Tag usage count doesn't match for {0}: expected {1}, actual {2}", tag, usages[tag.Id], tag.UsageCount);
				}
			}

		}

	}

}
