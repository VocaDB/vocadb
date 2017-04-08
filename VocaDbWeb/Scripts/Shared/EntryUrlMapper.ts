/// <reference path="../DataContracts/EntryRefContract.ts" />
/// <reference path="GlobalFunctions.ts" />

module vdb.utils {

	import cls = models;
	import dc = vdb.dataContracts;

    // Maps view URLs for common entry types.
    export class EntryUrlMapper {
    
        // URL to details view.
        // typeName: entry type name.
        // id: entry Id.
		public static details(typeName: string | cls.EntryType, id: number, urlFriendlyName?: string) {

			var prefix;

			if (typeof typeName  === "string") {
				typeName = cls.EntryType[typeName];
			}

			switch (typeName) {
				case cls.EntryType.Album:
					prefix = vdb.functions.mapAbsoluteUrl("/Al/" + id);
					break;
				case cls.EntryType.Artist:
					prefix = vdb.functions.mapAbsoluteUrl("/Ar/" + id);
					break;
				case cls.EntryType.ReleaseEvent:
					prefix = vdb.functions.mapAbsoluteUrl("/E/" + id);
					break;
				case cls.EntryType.Song:
					prefix = vdb.functions.mapAbsoluteUrl("/S/" + id);
					break;
				case cls.EntryType.SongList:
					prefix = vdb.functions.mapAbsoluteUrl("/L/" + id);
					break;
				case cls.EntryType.Tag:
					prefix = vdb.functions.mapAbsoluteUrl("/T/" + id);
					break;
				default:
					prefix = vdb.functions.mapAbsoluteUrl("/" + typeName + "/Details/" + id);
					break;
			}

			var urlFriendlyPart = urlFriendlyName ? "/" + urlFriendlyName : "";

			return prefix + urlFriendlyPart;

        }

        public static details_entry(entry: dc.EntryRefContract, slug?: string) {       		
            return EntryUrlMapper.details(entry.entryType, entry.id, slug);        
		}

		public static details_song(entry: dc.SongApiContract) {
			return EntryUrlMapper.details(cls.EntryType.Song, entry.id, entry.urlFriendlyName);
		}

		public static details_tag(id: number, slug?: string) {
			return EntryUrlMapper.details(cls.EntryType.Tag, id, slug);
		}

		public static details_tag_contract(tag: dc.TagBaseContract) {

			if (!tag)
				return null;

			if (!tag.id)
				return "/Tag/Details/" + tag.name; // Legacy URL, this will be removed

			return EntryUrlMapper.details(cls.EntryType.Tag, tag.id, tag.urlSlug);

		}
    
		public static details_user_byName(name: string) {
			return vdb.functions.mapAbsoluteUrl("/Profile/" + name);
		}
    }

}