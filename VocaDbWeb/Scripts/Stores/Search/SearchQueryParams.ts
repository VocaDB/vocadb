import { AlbumSortRule } from './AlbumSearchStore';
import { ArtistSortRule } from './ArtistSearchStore';
import { EventSortRule } from './EventSearchStore';
import { SearchType } from './SearchStore';
import { SongSortRule } from './SongSearchStore';
import { TagSortRule } from './TagSearchStore';

export interface AnythingSearchQueryParams {
	searchType?: SearchType.Anything;
	filter?: string;
	tag?: string;
	tagId?: number[];
	childTags?: boolean;
	pageSize?: number;
}

export interface AlbumSearchQueryParams {
	searchType: SearchType.Album;
	filter?: string;
	tag?: string;
	tagId?: number[];
	sort?: AlbumSortRule;
	artistId?: number[];
	childTags?: boolean;
	childVoicebanks?: boolean;
	discType?: string /* TODO: enum */;
	viewMode?: string /* TODO: enum */;
	pageSize?: number;
}

export interface ArtistSearchQueryParams {
	searchType: SearchType.Artist;
	filter?: string;
	tag?: string;
	tagId?: number[];
	sort?: ArtistSortRule;
	childTags?: boolean;
	artistType?: string /* TODO: enum */;
	pageSize?: number;
}

export interface EventSearchQueryParams {
	searchType: SearchType.ReleaseEvent;
	filter?: string;
	tag?: string;
	tagId?: number[];
	sort?: EventSortRule;
	artistId?: number[];
	childTags?: boolean;
	childVoicebanks?: boolean;
	eventCategory?: string;
	pageSize?: number;
}

export interface SongSearchQueryParams {
	searchType: SearchType.Song;
	filter?: string;
	tag?: string;
	tagId?: number[];
	sort?: SongSortRule;
	artistId?: number[];
	childTags?: boolean;
	childVoicebanks?: boolean;
	eventId?: number;
	songType?: string /* TODO: enum */;
	onlyWithPVs?: boolean;
	onlyRatedSongs?: boolean;
	since?: number;
	minScore?: number;
	viewMode?: string /* TODO: enum */;
	autoplay?: boolean;
	shuffle?: boolean;
	pageSize?: number;
}

export interface TagSearchQueryParams {
	searchType: SearchType.Tag;
	filter?: string;
	sort?: TagSortRule;
	pageSize?: number;
}

type SearchQueryParams =
	| AnythingSearchQueryParams
	| AlbumSearchQueryParams
	| ArtistSearchQueryParams
	| EventSearchQueryParams
	| SongSearchQueryParams
	| TagSearchQueryParams;

export default SearchQueryParams;
