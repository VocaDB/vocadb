
//module vdb.knockoutExtensions {

	import rep = vdb.repositories;

    export interface AutoCompleteParams {

		acceptSelection?: (id: number, term: string, itemType?: string) => void;

		// String for creating a custom item (item with no entry), with {0} as the placeholder for entry type name.
		// If this is null (default), no custom items can be created this way.
		createCustomItem?: string;

		// String for creating a new entry, with {0} as the placeholder for entry type name.
		// If this is null (default), no new entries can be created this way.
		createNewItem?: string;

		height?: number;

		// Ignore entry by this ID
		ignoreId?: number;

	}

	export interface AutoCompleteParamsGeneric<TContract, TQueryParams> extends AutoCompleteParams {

		extraQueryParams?: TQueryParams;
		
		// Callback for filtering out an item. If this callback returns false, the item will not be selectable. No filtering is done if this callback is null.
		filter?: (contract: TContract) => boolean;

	}

	export interface AlbumAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.AlbumContract, rep.AlbumQueryParams> { }

	export interface ArtistAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.ArtistContract, rep.ArtistQueryParams> { }

	export interface SongAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.SongContract, rep.SongQueryParams> { }

//}

