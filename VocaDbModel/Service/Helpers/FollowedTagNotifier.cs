using NHibernate.Exceptions;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Tags;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Service.Helpers;

public class FollowedTagNotifier
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();

	private string CreateMessageBody(Tag[] followedArtists, User user, IEntryWithNames entry, IEntryLinkFactory entryLinkFactory, bool markdown,
		string? entryTypeName)
	{
		var entryName = entry.Names.SortNames[user.DefaultLanguageSelection];
		var url = entryLinkFactory.GetFullEntryUrl(entry);

		string entryLink;
		if (markdown)
		{
			entryLink = MarkdownHelper.CreateMarkdownLink(url, entryName);
		}
		else
		{
			entryLink = $"{entryName} ( {url} )";
		}

		string msg;

		if (followedArtists.Length == 1)
		{
			var artistName = followedArtists.First().TranslatedName[user.DefaultLanguageSelection];
			msg = $"A new {entryTypeName}, '{entryLink}', tagged with {artistName} was just added.";
		}
		else
		{
			msg = $"A new {entryTypeName}, '{entryLink}', tagged with multiple tags you're following was just added.";
		}

		msg += "\nYou're receiving this notification because you're following the tag(s).";
		return msg;
	}

	/// <summary>
	/// Sends notifications
	/// </summary>
	/// <param name="ctx">Repository context. Cannot be null.</param>
	/// <param name="entry">Entry that was created. Cannot be null.</param>
	/// <param name="artists">List of artists for the entry. Cannot be null.</param>
	/// <param name="creator">User who created the entry. The creator will be excluded from all notifications. Cannot be null.</param>
	/// <param name="entryLinkFactory">Factory for creating links to entries. Cannot be null.</param>
	public async Task SendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry,
		IReadOnlyCollection<Tag> tags, int[] ignoreUsers, IEntryLinkFactory entryLinkFactory,
		IEnumTranslations enumTranslations)
	{
		ParamIs.NotNull(() => ctx);
		ParamIs.NotNull(() => entry);
		ParamIs.NotNull(() => tags);
		ParamIs.NotNull(() => ignoreUsers);
		ParamIs.NotNull(() => entryLinkFactory);

		if (!tags.Any())
			return;

		try
		{
			await DoSendNotificationsAsync(ctx, entry, tags, ignoreUsers, entryLinkFactory, enumTranslations);
		}
		catch (GenericADOException x)
		{
			s_log.Error(x, "Unable to send notifications");
		}
	}

	private async Task DoSendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry,
		IReadOnlyCollection<Tag> tags, int[] ignoreUsers, IEntryLinkFactory entryLinkFactory,
		IEnumTranslations enumTranslations)
	{
		var coll = tags.Distinct().ToArray();
		var tagIds = coll.Select(a => a.Id).ToArray();

		s_log.Info("Sending notifications for {0} tags", tagIds.Length);

		// Get users with less than maximum number of unread messages, following any of the tags
		var usersWithTagsArr = await ctx.Query<TagForUser>()
			.Where(afu =>
				tagIds.Contains(afu.Tag.Id))
			.Select(afu => new
			{
				UserId = afu.User.Id,
				TagId = afu.Tag.Id
			})
			.VdbToListAsync();

		var usersWithTags = usersWithTagsArr
			.GroupBy(afu => afu.UserId)
			.ToDictionary(afu => afu.Key, afu => afu.Select(a => a.TagId));

		var userIds = usersWithTags.Keys.Except(ignoreUsers).ToArray();

		s_log.Debug("Found {0} users subscribed to tags", userIds.Length);

		if (!userIds.Any())
		{
			s_log.Info("No users subscribed to tags - skipping.");
			return;
		}

		var entryTypeNames = enumTranslations.Translations<EntryType>();
		var users = await ctx.Query<User>()
			.WhereIsActive()
			.WhereIdIn(userIds)
			.Where(u => u.ReceivedMessages.Count(m => m.Inbox == UserInboxType.Notifications && !m.Read) < u.Options.UnreadNotificationsToKeep)
			.VdbToListAsync();

		foreach (var user in users)
		{
			var tagIdsForUser = new HashSet<int>(usersWithTags[user.Id]);
			var followedTags = coll.Where(a => tagIdsForUser.Contains(a.Id)).ToArray();

			if (followedTags.Length == 0)
				continue;

			string title;

			var entryTypeName = entryTypeNames.GetName(entry.EntryType, CultureHelper.GetCultureOrDefault(user.LanguageOrLastLoginCulture)).ToLowerInvariant();
			var msg = CreateMessageBody(followedTags, user, entry, entryLinkFactory, true, entryTypeName);

			if (followedTags.Length == 1)
			{
				var artistName = followedTags.First().TranslatedName[user.DefaultLanguageSelection];
				title = $"New {entryTypeName} tagged with {artistName}";
			}
			else
			{
				title = $"New {entryTypeName}";
			}

			var notification = user.CreateNotification(title, msg);
			await ctx.SaveAsync(notification);
		}

		s_log.Info($"Sent notifications to {users.Count} users");
	}
}
