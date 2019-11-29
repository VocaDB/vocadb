/// <reference path="../DataContracts/EntryRefContract.ts" />
/// <reference path="GlobalFunctions.ts" />

import EntryRefContract from '../DataContracts/EntryRefContract';
import EntryType from '../Models/EntryType';
import { mapAbsoluteUrl } from './GlobalFunctions';
import SongApiContract from '../DataContracts/Song/SongApiContract';
import TagBaseContract from '../DataContracts/Tag/TagBaseContract';

//module vdb.utils {

    // Maps view URLs for common entry types.
    export default class EntryUrlMapper {
    
        // URL to details view.
        // typeName: entry type name.
        // id: entry Id.
		public static details(typeName: string | EntryType, id: number, urlFriendlyName?: string) {

			var prefix;

			if (typeof typeName  === "string") {
				typeName = EntryType[typeName];
			}

			switch (typeName) {
				case EntryType.Album:
					prefix = mapAbsoluteUrl("/Al/" + id);
					break;
				case EntryType.Artist:
					prefix = mapAbsoluteUrl("/Ar/" + id);
					break;
				case EntryType.ReleaseEvent:
					prefix = mapAbsoluteUrl("/E/" + id);
					break;
				case EntryType.ReleaseEventSeries:
					prefix = mapAbsoluteUrl("/Es/" + id);
					break;
				case EntryType.Song:
					prefix = mapAbsoluteUrl("/S/" + id);
					break;
				case EntryType.SongList:
					prefix = mapAbsoluteUrl("/L/" + id);
					break;
				case EntryType.Tag:
					prefix = mapAbsoluteUrl("/T/" + id);
					break;
				default:
					prefix = mapAbsoluteUrl("/" + typeName + "/Details/" + id);
					break;
			}

			var urlFriendlyPart = urlFriendlyName ? "/" + urlFriendlyName : "";

			return prefix + urlFriendlyPart;

        }

        public static details_entry(entry: EntryRefContract, slug?: string) {       		
            return EntryUrlMapper.details(entry.entryType, entry.id, slug);        
		}

		public static details_song(entry: SongApiContract) {
			return EntryUrlMapper.details(EntryType.Song, entry.id, entry.urlFriendlyName);
		}

		public static details_tag(id: number, slug?: string) {
			return EntryUrlMapper.details(EntryType.Tag, id, slug);
		}

		public static details_tag_contract(tag: TagBaseContract) {

			if (!tag)
				return null;

			if (!tag.id)
				return "/Tag/Details/" + tag.name; // Legacy URL, this will be removed

			return EntryUrlMapper.details(EntryType.Tag, tag.id, tag.urlSlug);

		}
    
		public static details_user_byName(name: string) {
			return mapAbsoluteUrl("/Profile/" + name);
		}
    }

//}