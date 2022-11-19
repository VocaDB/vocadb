import { EntryRefContract } from '@/DataContracts/EntryRefContract';
import { EntryTypeAndSubTypeContract } from '@/DataContracts/EntryTypeAndSubTypeContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { TagApiContract } from '@/DataContracts/Tag/TagApiContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { EntryType } from '@/Models/EntryType';
import { SearchType } from '@/Stores/Search/SearchStore';
import qs from 'qs';

// Maps view URLs for common entry types.
export class EntryUrlMapper {
	// URL to details view.
	// typeName: entry type name.
	// id: entry Id.
	static details(
		typeName: EntryType,
		id: number,
		urlFriendlyName?: string,
	): string {
		var prefix;

		switch (typeName) {
			case EntryType.Album:
				prefix = `/Al/${id}`;
				break;
			case EntryType.Artist:
				prefix = `/Ar/${id}`;
				break;
			case EntryType.DiscussionTopic:
				prefix = `/discussion/topics/${id}`;
				break;
			case EntryType.ReleaseEvent:
				prefix = `/E/${id}`;
				break;
			case EntryType.ReleaseEventSeries:
				prefix = `/Es/${id}`;
				break;
			case EntryType.Song:
				prefix = `/S/${id}`;
				break;
			case EntryType.SongList:
				prefix = `/L/${id}`;
				break;
			case EntryType.Tag:
				prefix = `/T/${id}`;
				break;
			case EntryType.User:
				prefix = `/User/Details/${id}`;
				break;
			case EntryType.Venue:
				prefix = `/Venue/Details/${id}`;
				break;
			default:
				prefix = `/${typeName}/Details/${id}`;
				break;
		}

		var urlFriendlyPart = urlFriendlyName ? `/${urlFriendlyName}` : '';

		return prefix + urlFriendlyPart;
	}

	static details_entry(entry: EntryRefContract, slug?: string): string {
		return EntryUrlMapper.details(entry.entryType, entry.id, slug);
	}

	static details_song(entry: SongApiContract): string {
		return EntryUrlMapper.details(
			EntryType.Song,
			entry.id,
			entry.urlFriendlyName,
		);
	}

	static details_tag(id: number, slug?: string): string {
		return EntryUrlMapper.details(EntryType.Tag, id, slug);
	}

	static details_tag_contract(
		tag: TagBaseContract | TagApiContract | undefined,
	): string | undefined {
		if (!tag) return undefined;

		if (!tag.id) return `/Tag/Details/${tag.name}`; // Legacy URL, this will be removed

		return EntryUrlMapper.details(EntryType.Tag, tag.id, tag.urlSlug);
	}

	static details_user_byName(name?: string): string {
		return `/Profile/${name}`;
	}

	static index = (fullEntryType: EntryTypeAndSubTypeContract): string => {
		switch (fullEntryType.entryType) {
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
