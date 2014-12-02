/// <reference path="../DataContracts/EntryRefContract.ts" />
/// <reference path="GlobalFunctions.ts" />

module vdb.utils {

    // Maps view URLs for common entry types.
    export class EntryUrlMapper {
    
        // URL to details view.
        // typeName: entry type name.
        // id: entry Id.
		public static details(typeName: string, id: number) {

			switch (typeName.toLowerCase()) {
				case "album":
					return vdb.functions.mapAbsoluteUrl("/Al/" + id);
				case "artist":
					return vdb.functions.mapAbsoluteUrl("/Ar/" + id);
				case "song":
					return vdb.functions.mapAbsoluteUrl("/S/" + id);
			}

            return vdb.functions.mapAbsoluteUrl("/" + typeName + "/Details/" + id);
        }

        public static details_entry(entry: vdb.dataContracts.EntryRefContract) {            
            return EntryUrlMapper.details(entry.entryType, entry.id);        
		}

		public static details_tag_byName(name: string) {
			return vdb.functions.mapAbsoluteUrl("/Tag/Details/" + name);
		}
    
    }

}