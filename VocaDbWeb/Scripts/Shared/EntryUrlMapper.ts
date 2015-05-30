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
		public static details(typeName: string, id: number, urlFriendlyName?: string) {

			var prefix;

			switch (cls.EntryType[typeName]) {
				case cls.EntryType.Album:
					prefix = vdb.functions.mapAbsoluteUrl("/Al/" + id);
					break;
				case cls.EntryType.Artist:
					prefix = vdb.functions.mapAbsoluteUrl("/Ar/" + id);
					break;
				case cls.EntryType.ReleaseEvent:
					prefix = vdb.functions.mapAbsoluteUrl("/Event/Details/" + id);
					break;
				case cls.EntryType.Song:
					prefix = vdb.functions.mapAbsoluteUrl("/S/" + id);
					break;
				case cls.EntryType.Tag:
					prefix = vdb.functions.mapAbsoluteUrl("/Tag/DetailsById/" + id);
					break;
				default:
					prefix = vdb.functions.mapAbsoluteUrl("/" + typeName + "/Details/" + id);
					break;
			}

			var urlFriendlyPart = urlFriendlyName ? "/" + urlFriendlyName : "";

			return prefix + urlFriendlyPart;

        }

        public static details_entry(entry: dc.EntryRefContract) {       		
            return EntryUrlMapper.details(entry.entryType, entry.id);        
		}

		public static details_song(entry: dc.SongApiContract) {
			return EntryUrlMapper.details("Song", entry.id, entry.urlFriendlyName);
		}

		public static details_tag_byName(name: string) {
			return vdb.functions.mapAbsoluteUrl("/Tag/Details/" + name);
		}
    
		public static details_user_byName(name: string) {
			return vdb.functions.mapAbsoluteUrl("/User/Profile/" + name);
		}
    }

}