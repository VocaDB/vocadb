import EntryRefContract from '@DataContracts/EntryRefContract';
import EntryTypeAndSubTypeContract from '@DataContracts/EntryTypeAndSubTypeContract';
import SongApiContract from '@DataContracts/Song/SongApiContract';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import TagBaseContract from '@DataContracts/Tag/TagBaseContract';
import EntryType from '@Models/EntryType';
import { SearchType } from '@Stores/Search/SearchStore';
import qs from 'qs';

import functions from './GlobalFunctions';

// Maps view URLs for common entry types.
export default class EntryUrlMapper {
	// URL to details view.
	// typeName: entry type name.
	// id: entry Id.
	public static details(
		typeName: string | EntryType,
		id: number,
		urlFriendlyName?: string,
	): string {
		var prefix;

		if (typeof typeName === 'string') {
			typeName = EntryType[typeName as keyof typeof EntryType];
		}

		switch (typeName) {
			case EntryType.Album:
				prefix = functions.mapAbsoluteUrl(`/Al/${id}`);
				break;
			case EntryType.Artist:
				prefix = functions.mapAbsoluteUrl(`/Ar/${id}`);
				break;
			case EntryType.DiscussionTopic:
				prefix = functions.mapAbsoluteUrl(`/discussion/topics/${id}`);
				break;
			case EntryType.ReleaseEvent:
				prefix = functions.mapAbsoluteUrl(`/E/${id}`);
				break;
			case EntryType.ReleaseEventSeries:
				prefix = functions.mapAbsoluteUrl(`/Es/${id}`);
				break;
			case EntryType.Song:
				prefix = functions.mapAbsoluteUrl(`/S/${id}`);
				break;
			case EntryType.SongList:
				prefix = functions.mapAbsoluteUrl(`/L/${id}`);
				break;
			case EntryType.Tag:
				prefix = functions.mapAbsoluteUrl(`/T/${id}`);
				break;
			case EntryType.User:
				prefix = functions.mapAbsoluteUrl(`/User/Details/${id}`);
				break;
			case EntryType.Venue:
				prefix = functions.mapAbsoluteUrl(`/Venue/Details/${id}`);
				break;
			default:
				prefix = functions.mapAbsoluteUrl(`/${typeName}/Details/${id}`);
				break;
		}

		var urlFriendlyPart = urlFriendlyName ? '/' + urlFriendlyName : '';

		return prefix + urlFriendlyPart;
	}

	public static details_entry(entry: EntryRefContract, slug?: string): string {
		return EntryUrlMapper.details(entry.entryType, entry.id, slug);
	}

	public static details_song(entry: SongApiContract): string {
		return EntryUrlMapper.details(
			EntryType.Song,
			entry.id,
			entry.urlFriendlyName,
		);
	}

	public static details_tag(id: number, slug?: string): string {
		return EntryUrlMapper.details(EntryType.Tag, id, slug);
	}

	public static details_tag_contract(
		tag: TagBaseContract | TagApiContract | undefined,
	): string | undefined {
		if (!tag) return undefined;

		if (!tag.id) return '/Tag/Details/' + tag.name; // Legacy URL, this will be removed

		return EntryUrlMapper.details(EntryType.Tag, tag.id, tag.urlSlug);
	}

	public static details_user_byName(name?: string): string {
		return functions.mapAbsoluteUrl('/Profile/' + name);
	}

	public static index = (
		fullEntryType: EntryTypeAndSubTypeContract,
	): string => {
		switch (EntryType[fullEntryType.entryType as keyof typeof EntryType]) {
			case EntryType.Artist:
				return `/Search?${qs.stringify({
					searchType: SearchType.Artist,
					artistType: fullEntryType.subType,
				})}`;

			case EntryType.Album:
				return `/Search?${qs.stringify({
					searchType: SearchType.Album,
					discType: fullEntryType.subType,
				})}`;

			case EntryType.Song:
				return `/Search?${qs.stringify({
					searchType: SearchType.Song,
					songType: fullEntryType.subType,
				})}`;

			case EntryType.ReleaseEvent:
				return `/Search?${qs.stringify({
					searchType: SearchType.ReleaseEvent,
				})}`;

			case EntryType.Tag:
				return `/Search?${qs.stringify({
					searchType: SearchType.Tag,
				})}`;

			default:
				return '';
		}
	};
}
