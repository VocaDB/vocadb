using NHibernate.Exceptions;
using NLog;
using VocaDb.Model.Database.Repositories;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Helpers;
using VocaDb.Model.Service.QueryableExtensions;
using VocaDb.Model.Service.Translations;

namespace VocaDb.Model.Service.Helpers;

public interface IFollowedArtistNotifier
{
	/// <summary>
	/// Sends notifications
	/// </summary>
	/// <param name="ctx">Repository context. Cannot be null.</param>
	/// <param name="entry">Entry that was created. Cannot be null.</param>
	/// <param name="artists">List of artists for the entry. Cannot be null.</param>
	/// <param name="creator">User who created the entry. The creator will be excluded from all notifications. Cannot be null.</param>
	Task<IReadOnlyCollection<User>> SendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator);
}

/// <summary>
/// Sends notifications to users based on artists they're following.
/// 
/// Notifications will not be sent to users with too many unread messages in their inbox.
/// This is to prevent flooding users with too many notifications.
/// </summary>
public class FollowedArtistNotifier : IFollowedArtistNotifier
{
	private static readonly Logger s_log = LogManager.GetCurrentClassLogger();
	private readonly IEntryLinkFactory _entryLinkFactory;
	private readonly IUserMessageMailer _mailer;
	private readonly IEnumTranslations _enumTranslations;
	private readonly IEntrySubTypeNameFactory _entrySubTypeNameFactory;

	public FollowedArtistNotifier(IEntryLinkFactory entryLinkFactory, IUserMessageMailer mailer,
		IEnumTranslations enumTranslations, IEntrySubTypeNameFactory entrySubTypeNameFactory)
	{
		_entryLinkFactory = entryLinkFactory;
		_mailer = mailer;
		_enumTranslations = enumTranslations;
		_entrySubTypeNameFactory = entrySubTypeNameFactory;
	}

	private string CreateMessageBody(Artist[] followedArtists, User user, IEntryWithNames entry, bool markdown,
		string? entryTypeName)
	{
		var entryName = entry.Names.SortNames[user.DefaultLanguageSelection];
		var url = _entryLinkFactory.GetFullEntryUrl(entry);

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
			msg = $"A new {entryTypeName}, '{entryLink}', by {artistName} was just added.";
		}
		else
		{
			msg = $"A new {entryTypeName}, '{entryLink}', by multiple artists you're following was just added.";
		}

		msg += "\nYou're receiving this notification because you're following the artist(s).";
		return msg;
	}

	public async Task<IReadOnlyCollection<User>> SendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator)
	{
		try
		{
			return await DoSendNotificationsAsync(ctx, entry, artists, creator);
		}
		catch (GenericADOException x)
		{
			s_log.Error(x, "Unable to send notifications");
			return Array.Empty<User>();
		}
	}

	private async Task<IReadOnlyCollection<User>> DoSendNotificationsAsync(IDatabaseContext ctx, IEntryWithNames entry, IEnumerable<Artist> artists, IUser creator)
	{
		ParamIs.NotNull(() => ctx);
		ParamIs.NotNull(() => entry);
		ParamIs.NotNull(() => artists);
		ParamIs.NotNull(() => creator);
		ParamIs.NotNull(() => _entryLinkFactory);
		ParamIs.NotNull(() => _mailer);

		var coll = artists.ToArray();
		var artistIds = coll.Select(a => a.Id).ToArray();

		s_log.Info("Sending notifications for {0} artists", artistIds.Length);

		// Get users with less than maximum number of unread messages, following any of the artists
		var usersWithArtistsArr = await ctx.Query<ArtistForUser>()
			.Where(afu =>
				artistIds.Contains(afu.Artist.Id)
				&& afu.User.Id != creator.Id
				&& afu.SiteNotifications)
			.Select(afu => new
			{
				UserId = afu.User.Id,
				ArtistId = afu.Artist.Id
			})
			.VdbToListAsync();

		var usersWithArtists = usersWithArtistsArr
			.GroupBy(afu => afu.UserId)
			.ToDictionary(afu => afu.Key, afu => afu.Select(a => a.ArtistId));

		var userIds = usersWithArtists.Keys;

		s_log.Debug("Found {0} users subscribed to artists", userIds.Count);

		if (!userIds.Any())
		{
			s_log.Info("No users found - skipping.");
			return Array.Empty<User>();
		}

		var entryTypeNames = _enumTranslations.Translations<EntryType>();
		var users = await ctx.Query<User>()
			.WhereIsActive()
			.WhereIdIn(userIds)
			.Where(u => u.ReceivedMessages.Count(m => m.Inbox == UserInboxType.Notifications && !m.Read) < u.Options.UnreadNotificationsToKeep)
			.VdbToListAsync();

		foreach (var user in users)
		{
			var artistIdsForUser = new HashSet<int>(usersWithArtists[user.Id]);
			var followedArtists = coll.Where(a => artistIdsForUser.Contains(a.Id)).ToArray();

			if (followedArtists.Length == 0)
				continue;

			string title;

			var culture = CultureHelper.GetCultureOrDefault(user.LanguageOrLastLoginCulture);
			var entryTypeName = entryTypeNames.GetName(entry.EntryType, culture).ToLowerInvariant();
			var entrySubType = _entrySubTypeNameFactory.GetEntrySubTypeName(entry, _enumTranslations, culture)?.ToLowerInvariant();

			if (!string.IsNullOrEmpty(entrySubType))
			{
				entryTypeName += $" ({entrySubType})";
			}

			var msg = CreateMessageBody(followedArtists, user, entry, true, entryTypeName);

			if (followedArtists.Length == 1)
			{
				var artistName = followedArtists.First().TranslatedName[user.DefaultLanguageSelection];
				title = $"New {entryTypeName} by {artistName}";
			}
			else
			{
				title = $"New {entryTypeName}";
			}

			var notification = new UserMessage(user, title, msg, false);
			user.Messages.Add(notification);
			await ctx.SaveAsync(notification);

			if (user.EmailOptions != UserEmailOptions.NoEmail && !string.IsNullOrEmpty(user.Email)
				&& followedArtists.Any(a => a.Users.Any(u => u.User.Equals(user) && u.EmailNotifications)))
			{
				await _mailer.SendEmailAsync(user.Email, user.Name, title, CreateMessageBody(followedArtists, user, entry, false, entryTypeName));
			}
		}

		s_log.Info($"Sent notifications to {users.Count} users");

		return users;
	}
}
