using System;
using System.Web.Mvc;
using VocaDb.Model;
using VocaDb.Model.DataContracts.Api;
using VocaDb.Model.DataContracts.Tags;
using VocaDb.Model.Domain;
using VocaDb.Model.Domain.Albums;
using VocaDb.Model.Domain.Artists;
using VocaDb.Model.Domain.Songs;
using VocaDb.Model.Domain.Users;
using VocaDb.Model.Utils;
using VocaDb.Model.Utils.Search;

namespace VocaDb.Web.Helpers
{

	public static class UrlHelperExtender
	{

		private static string EntryDetails(UrlHelper urlHelper, EntryType entryType, int id, string urlSlug)
		{

			switch (entryType)
			{
				case EntryType.DiscussionTopic:
					return urlHelper.Action("Index", "Discussion", new { clientPath = string.Format("topics/{0}", id) });

				case EntryType.ReleaseEvent:
					return urlHelper.Action("Details", "Event", new { id, slug = urlSlug });

				case EntryType.ReleaseEventSeries:
					return urlHelper.Action("SeriesDetails", "Event", new { id, slug = urlSlug });

				case EntryType.Tag:
					return urlHelper.Action("DetailsById", "Tag", new { id, slug = urlSlug });

				default:
					return urlHelper.Action("Details", entryType.ToString(), new { id });
			}

		}

		public static string EntryDetails(this UrlHelper urlHelper, IEntryBase entryBase, string urlSlug = null)
		{

			ParamIs.NotNull(() => entryBase);

			return EntryDetails(urlHelper, entryBase.EntryType, entryBase.Id, urlSlug);

		}

		public static string EntryDetails(this UrlHelper urlHelper, EntryForApiContract entry)
		{

			ParamIs.NotNull(() => entry);

			return EntryDetails(urlHelper, entry.EntryType, entry.Id, entry.UrlSlug);

		}

		public static string EntryIndex(this UrlHelper urlHelper, EntryTypeAndSubType fullEntryType)
		{

			SearchRouteParams searchRouteParams = null;
			switch (fullEntryType.EntryType)
			{
				case EntryType.Artist:
					searchRouteParams = new SearchRouteParams(EntryType.Artist) { artistType = EnumVal<ArtistType>.ParseSafe(fullEntryType.SubType) };
					break;
				case EntryType.Album:
					searchRouteParams = new SearchRouteParams(EntryType.Album) { discType = EnumVal<DiscType>.ParseSafe(fullEntryType.SubType) };
					break;
				case EntryType.Song:
					searchRouteParams = new SearchRouteParams(EntryType.Song) { songType = EnumVal<SongType>.ParseSafe(fullEntryType.SubType) };
					break;
				case EntryType.ReleaseEvent:
					searchRouteParams = new SearchRouteParams(EntryType.ReleaseEvent);
					break;
				case EntryType.Tag:
					searchRouteParams = new SearchRouteParams(EntryType.Tag);
					break;
			}

			if (searchRouteParams != null)
			{
				return urlHelper.Action("Index", "Search", searchRouteParams);
			}

			return "";

		}

		public static string SongDetails(this UrlHelper urlHelper, IEntryBase song, int? albumId = null)
		{

			ParamIs.NotNull(() => song);

			return urlHelper.Action("Details", "Song", new { id = song.Id, albumId });

		}

		public static string StaticResource(this UrlHelper urlHelper, string url)
		{
			return VocaUriBuilder.StaticResource(url);
		}

		public static string TagDetails(this UrlHelper urlHelper, TagBaseContract tagContract)
		{

			ParamIs.NotNull(() => tagContract);

			return EntryDetails(urlHelper, EntryType.Tag, tagContract.Id, tagContract.UrlSlug);

		}

		public static string TagUrlForEntryType<TSubType>(this UrlHelper urlHelper, EntryType entryType, TSubType subType)
			where TSubType : struct, Enum
		{
			return TagUrlForEntryType(urlHelper, EntryTypeAndSubType.Create(entryType, subType));
		}

		public static string TagUrlForEntryType(this UrlHelper urlHelper, EntryTypeAndSubType entryType)
		{
			return urlHelper.Action("DetailsByEntryType", "Tag", new { entryType = entryType.EntryType, subType = entryType.SubType });
		}

		public static string UserDetails(this UrlHelper urlHelper, IUser user)
		{

			ParamIs.NotNull(() => user);

			return urlHelper.Action("Profile", "User", new { id = user.Name });

		}

	}

}