import { PV } from "./pv";
import { TagUsage } from "./tag";
import { Contains, FilterUndefined } from "./utility";
import { WebLink } from "./weblink";

export enum SongFields {
	AdditonalNames,
	Albums,
	Artists,
	Lyrics,
	MainPictures,
	Names,
	PVs,
	ReleaseEvent,
	Tags,
	ThumbUrl,
	WebLinks,
	BPM,
	CultureCodes,
}

export enum TranslationType {
	Original = "Original",
	Romanized = "Romanized",
	Translation = "Translation",
}

export interface SongLyrics {
	cultureCodes?: string[]; // TODO: Nullable?
	id: number;
	source: string | undefined; // TODO: Nullable?
	translationType: TranslationType;
	url: string | undefined; // TODO: Nullable?
	value: string | undefined; // TODO: Nullable?
}

export interface MainPicture {
	mime?: string;
	name?: string;
	urlOriginal?: string;
	urlSmallThumb?: string;
	urlThumb?: string;
	urlTinyThumb?: string;
}

export enum ContentLanguage {
	Unspecified = "Unspecified",
	Japanese = "Japanese",
	Romaji = "Romaji",
	English = "English",
}

export interface Name {
	language: ContentLanguage;
	value: string | undefined; // TODO: Nullable?
}

export enum SongType {
	Unspecified = "Unspecified",
	Original = "Original",
	Remaster = "Remaster",
	Remix = "Remix",
	Cover = "Cover",
	Arrangement = "Arrangement",
	Instrumental = "Instrumental",
	Mashup = "Mashup",
	MusicPV = "MusicPV",
	DramaPV = "DramaPV",
	Live = "Live",
	Illustration = "Illustration",
	Other = "Other",
	Rearrangement = "Rearrangement",
	ShortVersion = "ShortVersion",
}

export enum EntryStatus {
	Draft = "Draft",
	Approved = "Approved",
	Finished = "Finished",
	Locked = "Locked",
}

export type Song<F = []> = FilterUndefined<{
	id: number;
	additionalNames: Contains<F, SongFields.AdditonalNames, string>;
	albums: Contains<F, SongFields.Albums, any>; // TODO: Types
	artists: Contains<F, SongFields.Artists, any>; // TODO: Types
	artistString: string | undefined; // TODO: Nullable?
	createDate: string;
	defaultName: string | undefined; // TODO: Nullable?
	deleted: boolean;
	favoritedTimes: number;
	lengthSeconds: number;
	lyrics: Contains<F, SongFields.Lyrics, SongLyrics[]>;
	mainPicture?: Contains<F, SongFields.MainPictures, MainPicture>;
	minMilliBpm?: Contains<F, SongFields.BPM, number>;
	maxMilliBpm?: Contains<F, SongFields.BPM, number>;
	mergedTo?: number;
	name: string | undefined; // TODO: Nullable?
	names: Contains<F, SongFields.Names, Name[]>;
	originalVersionId?: number;
	publishDate?: string;
	pvs: Contains<F, SongFields.PVs, PV[]>;
	pvServices: string;
	ratingScore: number;
	releaseEvents: Contains<F, SongFields.ReleaseEvent, any>; // TODO: Types
	songType: SongType;
	entryStatus: EntryStatus;
	tags: Contains<F, SongFields.Tags, TagUsage[]>;
	thumbUrl: Contains<F, SongFields.ThumbUrl, string>;
	version: number;
	webLinks: Contains<F, SongFields.WebLinks, WebLink[]>;
	cultureCodes: Contains<F, SongFields.CultureCodes, string[]>;
}>;

export type T = Song<
	[SongFields.AdditonalNames, SongFields.Lyrics, SongFields.PVs]
>;

export enum UserRating {
	Nothing = "Nothing",
	Like = "Like",
	Favorite = "Favorite",
}

export interface SongDetails {
	additionalNames: string;
	albums: any[]; // TODO: Types
	alternateVersions: Song[];
	artists: any[]; // TODO: Types
	artistString: string;
	canEditPersonalDescription: boolean; // TODO: Needed?
	canRemoveTagUsages: boolean; // TODO: Needed?
	commentCount: number;
	createDate: string;
	deleted: boolean;
	hits: number;
	latestComments: any[]; // TODO: Types
	likeCount: number;
	listCount: number;
	lyricsFromParents: any[]; // TODO: Types
	maxMilliBpm?: number;
	minMilliBpm?: number;
	notes: {
		english: string;
		original: string;
	};
	pools: any[]; // TODO: Types
	preferredLyrics: any[]; // TODO: Types
	pvs: PV[];
	releaseEvents: any[]; // TODO: Types
	song: Song<[SongFields.MainPictures]>;
	songTypeTag?: {
		id: number;
		name: string;
		urlSlug: string;
	};
	subjectsFromParents: any[]; // TODO: Types
	suggestions: Song<[SongFields.MainPictures]>;
	tags: TagUsage[];
	userRating: UserRating;
	weblinks: WebLink[];
	cultureCodes: string[];
}

