import { AlbumForApiContract } from '@/DataContracts/Album/AlbumForApiContract';
import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { CommentContract } from '@/DataContracts/CommentContract';
import { EnglishTranslatedStringContract } from '@/DataContracts/Globalization/EnglishTranslatedStringContract';
import { PVContract } from '@/DataContracts/PVs/PVContract';
import { ReleaseEventContract } from '@/DataContracts/ReleaseEvents/ReleaseEventContract';
import { ArtistForSongContract } from '@/DataContracts/Song/ArtistForSongContract';
import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongDetailsContract } from '@/DataContracts/Song/SongDetailsContract';
import { SongListBaseContract } from '@/DataContracts/SongListBaseContract';
import { TagBaseContract } from '@/DataContracts/Tag/TagBaseContract';
import { TagUsageForApiContract } from '@/DataContracts/Tag/TagUsageForApiContract';
import { WebLinkContract } from '@/DataContracts/WebLinkContract';
import { RelatedSitesHelper } from '@/Helpers/RelatedSitesHelper';
import { SongHelper } from '@/Helpers/SongHelper';
import { ArtistCategories } from '@/Models/Artists/ArtistCategories';
import { ContentFocus } from '@/Models/ContentFocus';
import { EntryStatus } from '@/Models/EntryStatus';
import { PVType } from '@/Models/PVs/PVType';
import { SongType } from '@/Models/Songs/SongType';

// Corresponds to the SongDetails class in C#.
export class SongDetailsForApi {
	readonly additionalNames: string;
	readonly albums: AlbumForApiContract[];
	readonly alternateVersions: SongApiContract[];
	readonly animators: ArtistForSongContract[];
	readonly artistString?: string;
	readonly bands: ArtistForSongContract[];
	readonly browsedAlbumId?: number;
	readonly canEditPersonalDescription: boolean;
	readonly canRemoveTagUsages: boolean;
	readonly commentCount: number;
	readonly createDate: string;
	readonly cultureCodes: string[];
	readonly deleted: boolean;
	readonly draft: boolean;
	readonly favoritedTimes: number;
	readonly hits: number;
	readonly id: number;
	readonly illustrators?: ArtistForSongContract[];
	readonly jsonModel: SongDetailsAjax;
	readonly latestComments: CommentContract[];
	readonly length: number;
	readonly likedTimes: number;
	readonly listCount: number;
	readonly lyrics: LyricsForSongContract[];
	readonly maxMilliBpm?: number;
	readonly mergedTo?: SongApiContract;
	readonly minMilliBpm?: number;
	readonly name: string;
	readonly notes: EnglishTranslatedStringContract;
	readonly originalPVs: PVContract[];
	readonly originalVersion?: SongApiContract;
	readonly otherArtists: ArtistForSongContract[];
	readonly otherPVs: PVContract[];
	readonly performers: ArtistForSongContract[];
	readonly personalDescriptionAuthor?: ArtistApiContract;
	readonly personalDescriptionText?: string;
	readonly pools: SongListBaseContract[];
	readonly primaryPV?: PVContract;
	readonly producers: ArtistForSongContract[];
	readonly publishDate?: string;
	readonly ratingScore: number;
	readonly releaseEvent?: ReleaseEventContract;
	readonly songType: SongType;
	readonly songTypeTag: TagBaseContract;
	readonly status: EntryStatus;
	readonly subject: ArtistForSongContract[];
	readonly suggestions: SongApiContract[];
	readonly tags: TagUsageForApiContract[];
	readonly userRating: string /* TODO: enum */;
	readonly webLinks: WebLinkContract[];

	constructor(
		readonly contract: SongDetailsContract,
		primaryPV: PVContract | undefined,
	) {
		this.additionalNames = contract.additionalNames;
		this.albums = contract.albums;
		this.alternateVersions = contract.alternateVersions.filter(
			(a) => a.songType !== SongType.Original,
		);
		this.artistString = contract.artistString;
		this.browsedAlbumId = contract.album?.id;
		this.canEditPersonalDescription = contract.canEditPersonalDescription;
		this.canRemoveTagUsages = contract.canRemoveTagUsages;
		this.commentCount = contract.commentCount;
		this.createDate = contract.createDate;
		this.cultureCodes = contract.cultureCodes;
		this.deleted = contract.deleted;
		this.draft = contract.song.status === EntryStatus.Draft;
		this.favoritedTimes = contract.song.favoritedTimes!;
		this.hits = contract.hits;
		this.id = contract.song.id;
		this.latestComments = contract.latestComments;
		this.length = contract.song.lengthSeconds;
		this.likedTimes = contract.likeCount;
		this.listCount = contract.listCount;
		this.lyrics = contract.lyricsFromParents;
		this.maxMilliBpm = contract.maxMilliBpm;
		this.mergedTo = contract.mergedTo;
		this.minMilliBpm = contract.minMilliBpm;
		this.name = contract.song.name;
		this.notes = contract.notes;

		this.originalVersion =
			contract.song.songType !== SongType.Original
				? contract.originalVersion
				: undefined;

		this.personalDescriptionAuthor = contract.personalDescriptionAuthor;
		this.personalDescriptionText = contract.personalDescriptionText;
		this.pools = contract.pools;
		this.publishDate = contract.song.publishDate;
		this.ratingScore = contract.song.ratingScore;
		this.releaseEvent = contract.releaseEvent;
		this.songType = contract.song.songType;
		this.songTypeTag = contract.songTypeTag;
		this.status = contract.song.status;
		this.suggestions = contract.suggestions;
		this.tags = contract.tags;
		this.userRating = contract.userRating;
		this.webLinks = contract.webLinks;

		const contentFocus = SongHelper.getContentFocus(this.songType);

		this.animators = contract.artists.filter((artist) =>
			artist.categories
				.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Animator),
		);

		this.bands = contract.artists.filter((artist) =>
			artist.categories
				.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Band),
		);

		this.illustrators =
			contentFocus === ContentFocus.Illustration
				? contract.artists.filter((artist) =>
						artist.categories
							.split(',')
							.map((category) => category.trim())
							.includes(ArtistCategories.Illustrator),
				  )
				: undefined;

		// OPTIMIZE
		this.otherArtists = contract.artists.filter(
			(artist) =>
				artist.categories
					.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Circle) ||
				artist.categories
					.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Label) ||
				artist.categories
					.split(',')
					.map((category) => category.trim())
					.includes(ArtistCategories.Other) ||
				(contentFocus !== ContentFocus.Illustration &&
					artist.categories
						.split(',')
						.map((category) => category.trim())
						.includes(ArtistCategories.Illustrator)),
		);

		this.performers = contract.artists.filter((artist) =>
			artist.categories
				.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Vocalist),
		);

		this.producers = contract.artists.filter((artist) =>
			artist.categories
				.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Producer),
		);

		const subjectsForThis = contract.artists.filter((artist) =>
			artist.categories
				.split(',')
				.map((category) => category.trim())
				.includes(ArtistCategories.Subject),
		);
		this.subject =
			subjectsForThis.length > 0
				? subjectsForThis
				: contract.subjectsFromParents;

		this.originalPVs = contract.pvs.filter(
			(pv) => pv.pvType === PVType.Original,
		);

		this.otherPVs = contract.pvs.filter((pv) => pv.pvType !== PVType.Original);

		this.primaryPV = primaryPV;

		this.jsonModel = new SongDetailsAjax(
			this,
			contract.preferredLyrics,
			contract.song.version!,
		);
	}
}

export class SongDetailsAjax {
	readonly id: number;
	readonly latestComments: CommentContract[];
	readonly linkedPages?: string[];
	readonly originalVersion?: SongApiContract;
	readonly selectedLyricsId: number;
	readonly selectedPvId: number;
	readonly personalDescriptionAuthor?: ArtistApiContract;
	readonly personalDescriptionText?: string;
	readonly songType: SongType;
	readonly tagUsages: TagUsageForApiContract[];
	readonly userRating: string;
	readonly version: number;

	constructor(
		model: SongDetailsForApi,
		preferredLyrics: LyricsForSongContract | undefined,
		version: number,
	) {
		this.id = model.id;
		this.userRating = model.userRating;
		this.latestComments = model.latestComments;
		this.originalVersion = model.originalVersion;
		this.personalDescriptionAuthor = model.personalDescriptionAuthor;
		this.personalDescriptionText = model.personalDescriptionText;
		this.version = version;

		this.selectedLyricsId = preferredLyrics?.id ?? 0;
		this.selectedPvId = model.primaryPV?.id ?? 0;
		this.songType = model.songType;
		this.tagUsages = model.tags;

		this.linkedPages = model.webLinks
			.map((w) => w.url)
			.filter(RelatedSitesHelper.isRelatedSite);
	}
}
