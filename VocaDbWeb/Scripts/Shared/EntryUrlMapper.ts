/// <reference path="../DataContracts/EntryRefContract.ts" />
/// <reference path="GlobalFunctions.ts" />

module vdb.utils {

	import dc = vdb.dataContracts;

    // Maps view URLs for common entry types.
    export class EntryUrlMapper {
    
        // URL to details view.
        // typeName: entry type name.
        // id: entry Id.
		public static details(typeName: string, id: number, urlFriendlyName?: string) {

			var prefix;

			switch (typeName.toLowerCase()) {
				case "album":
					prefix = vdb.functions.mapAbsoluteUrl("/Al/" + id);
					break;
				case "artist":
					prefix = vdb.functions.mapAbsoluteUrl("/Ar/" + id);
					break;
				case "song":
					prefix = vdb.functions.mapAbsoluteUrl("/S/" + id);
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
    
    }

}