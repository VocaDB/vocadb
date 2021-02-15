#nullable disable

using System;
using Microsoft.AspNetCore.Mvc;
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
	public static class UrlHelperExtensions
	{
		private static string EntryDetails(IUrlHelper urlHelper, EntryType entryType, int id, string urlSlug) => entryType switch
		{
			EntryType.DiscussionTopic => urlHelper.Action("Index", "Discussion", new { clientPath = $"topics/{id}" }),
			EntryType.ReleaseEvent => urlHelper.Action("Details", "Event", new { id, slug = urlSlug }),
			EntryType.ReleaseEventSeries => urlHelper.Action("SeriesDetails", "Event", new { id, slug = urlSlug }),
			EntryType.Tag => urlHelper.Action("DetailsById", "Tag", new { id, slug = urlSlug }),
			_ => urlHelper.Action("Details", entryType.ToString(), new { id }),
		};

		public static string EntryDetails(this IUrlHelper urlHelper, IEntryBase entryBase, string urlSlug = null)
		{
			ParamIs.NotNull(() => entryBase);

			return EntryDetails(urlHelper, entryBase.EntryType, entryBase.Id, urlSlug);
		}

		public static string EntryDetails(this IUrlHelper urlHelper, EntryForApiContract entry)
		{
			ParamIs.NotNull(() => entry);

			return EntryDetails(urlHelper, entry.EntryType, entry.Id, entry.UrlSlug);
		}

		public static string EntryIndex(this IUrlHelper urlHelper, EntryTypeAndSubType fullEntryType)
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

		public static string SongDetails(this IUrlHelper urlHelper, IEntryBase song, int? albumId = null)
		{
			ParamIs.NotNull(() => song);

			return urlHelper.Action("Details", "Song", new { id = song.Id, albumId });
		}

		public static string StaticResource(this IUrlHelper urlHelper, string url)
		{
			return VocaUriBuilder.StaticResource(url);
		}

		public static string TagDetails(this IUrlHelper urlHelper, TagBaseContract tagContract)
		{
			ParamIs.NotNull(() => tagContract);

			return EntryDetails(urlHelper, EntryType.Tag, tagContract.Id, tagContract.UrlSlug);
		}

		public static string TagUrlForEntryType<TSubType>(this IUrlHelper urlHelper, EntryType entryType, TSubType subType)
			where TSubType : struct, Enum
		{
			return TagUrlForEntryType(urlHelper, EntryTypeAndSubType.Create(entryType, subType));
		}

		public static string TagUrlForEntryType(this IUrlHelper urlHelper, EntryTypeAndSubType entryType)
		{
			return urlHelper.Action("DetailsByEntryType", "Tag", new { entryType = entryType.EntryType, subType = entryType.SubType });
		}

		public static string UserDetails(this IUrlHelper urlHelper, IUser user)
		{
			ParamIs.NotNull(() => user);

			return urlHelper.Action("Profile", "User", new { id = user.Name });
		}
	}
}