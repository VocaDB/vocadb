
module vdb.knockoutExtensions {

	import rep = vdb.repositories;

    export interface AutoCompleteParams {

		acceptSelection?: (id: number, term: string, itemType?: string) => void;

		allowCreateNew?: boolean;

		createCustomItem?: string;

		createNewItem?: string;

		height?: number;

		// Ignore entry by this ID
		ignoreId?: number;

	}

	export interface AutoCompleteParamsGeneric<TContract, TQueryParams> extends AutoCompleteParams {

		extraQueryParams?: TQueryParams;

		filter?: (contract: TContract) => boolean;

	}

	export interface AlbumAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.AlbumContract, rep.AlbumQueryParams> { }

	export interface ArtistAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.ArtistContract, rep.ArtistQueryParams> { }

	export interface SongAutoCompleteParams extends AutoCompleteParamsGeneric<vdb.dataContracts.SongContract, rep.SongQueryParams> { }

}

