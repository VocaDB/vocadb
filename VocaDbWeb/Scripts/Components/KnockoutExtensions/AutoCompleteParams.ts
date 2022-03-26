import AlbumContract from '@DataContracts/Album/AlbumContract';
import ArtistContract from '@DataContracts/Artist/ArtistContract';
import SongContract from '@DataContracts/Song/SongContract';
import { AlbumQueryParams } from '@Repositories/AlbumRepository';
import { ArtistQueryParams } from '@Repositories/ArtistRepository';
import { SongQueryParams } from '@Repositories/SongRepository';

export interface AutoCompleteParams {
	acceptSelection?: (id?: number, term?: string, itemType?: string) => void;

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

export interface AutoCompleteParamsGeneric<TContract, TQueryParams>
	extends AutoCompleteParams {
	extraQueryParams?: TQueryParams;

	// Callback for filtering out an item. If this callback returns false, the item will not be selectable. No filtering is done if this callback is null.
	filter?: (contract: TContract) => boolean;
}

export interface AlbumAutoCompleteParams
	extends AutoCompleteParamsGeneric<AlbumContract, AlbumQueryParams> {}

export interface ArtistAutoCompleteParams
	extends AutoCompleteParamsGeneric<ArtistContract, ArtistQueryParams> {}

export interface SongAutoCompleteParams
	extends AutoCompleteParamsGeneric<SongContract, SongQueryParams> {}
