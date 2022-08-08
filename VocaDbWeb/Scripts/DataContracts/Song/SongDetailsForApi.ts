import PVHelper from '@/Helpers/PVHelper';
import RelatedSitesHelper from '@/Helpers/RelatedSitesHelper';
import SongHelper from '@/Helpers/SongHelper';
import ArtistCategories from '@/Models/Artists/ArtistCategories';
import ContentFocus from '@/Models/ContentFocus';
import EntryStatus from '@/Models/EntryStatus';
import PVType from '@/Models/PVs/PVType';
import SongType from '@/Models/Songs/SongType';

import AlbumForApiContract from '../Album/AlbumForApiContract';
import ArtistApiContract from '../Artist/ArtistApiContract';
import CommentContract from '../CommentContract';
import EnglishTranslatedStringContract from '../Globalization/EnglishTranslatedStringContract';
import PVContract from '../PVs/PVContract';
import ReleaseEventContract from '../ReleaseEvents/ReleaseEventContract';
import SongListBaseContract from '../SongListBaseContract';
import TagBaseContract from '../Tag/TagBaseContract';
import TagUsageForApiContract from '../Tag/TagUsageForApiContract';
import WebLinkContract from '../WebLinkContract';
import ArtistForSongContract from './ArtistForSongContract';
import LyricsForSongContract from './LyricsForSongContract';
import SongApiContract from './SongApiContract';
import SongDetailsContract from './SongDetailsContract';

// Corresponds to the SongDetails class in C#.
export default class SongDetailsForApi {
	public readonly additionalNames: string;
	public readonly albums: AlbumForApiContract[];
	public readonly alternateVersions: SongApiContract[];
	public readonly animators: ArtistForSongContract[];
	public readonly artistString?: string;
	public readonly bands: ArtistForSongContract[];
	public readonly browsedAlbumId?: number;
	public readonly canEditPersonalDescription: boolean;
	public readonly canRemoveTagUsages: boolean;
	public readonly commentCount: number;
	public readonly createDate: Date;
	public readonly deleted: boolean;
	public readonly draft: boolean;
	public readonly favoritedTimes: number;
	public readonly hits: number;
	public readonly id: number;
	public readonly illustrators?: ArtistForSongContract[];
	public readonly jsonModel: SongDetailsAjax;
	public readonly latestComments: CommentContract[];
	public readonly length: number;
	public readonly likedTimes: number;
	public readonly listCount: number;
	public readonly lyrics: LyricsForSongContract[];
	public readonly maxMilliBpm?: number;
	public readonly mergedTo?: SongApiContract;
	public readonly minMilliBpm?: number;
	public readonly name: string;
	public readonly notes: EnglishTranslatedStringContract;
	public readonly originalPVs: PVContract[];
	public readonly originalVersion?: SongApiContract;
	public readonly otherArtists: ArtistForSongContract[];
	public readonly otherPVs: PVContract[];
	public readonly performers: ArtistForSongContract[];
	public readonly personalDescriptionAuthor?: ArtistApiContract;
	public readonly personalDescriptionText?: string;
	public readonly pools: SongListBaseContract[];
	public readonly primaryPV?: PVContract;
	public readonly producers: ArtistForSongContract[];
	public readonly publishDate?: string;
	public readonly ratingScore: number;
	public readonly releaseEvent?: ReleaseEventContract;
	public readonly songType: SongType;
	public readonly songTypeTag: TagBaseContract;
	public readonly status: string /* TODO: enum */;
	public readonly subject: ArtistForSongContract[];
	public readonly suggestions: SongApiContract[];
	public readonly tags: TagUsageForApiContract[];
	public readonly userRating: string /* TODO: enum */;
	public readonly webLinks: WebLinkContract[];

	public constructor(public readonly contract: SongDetailsContract) {
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
		this.deleted = contract.deleted;
		this.draft = contract.song.status === EntryStatus[EntryStatus.Draft];
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
			(pv) => pv.pvType === PVType[PVType.Original],
		);

		this.otherPVs = contract.pvs.filter(
			(pv) => pv.pvType !== PVType[PVType.Original],
		);

		this.primaryPV = PVHelper.primaryPV(contract.pvs);

		this.jsonModel = new SongDetailsAjax(
			this,
			contract.preferredLyrics,
			contract.song.version!,
		);
	}
}

export class SongDetailsAjax {
	public readonly id: number;
	public readonly latestComments: CommentContract[];
	public readonly linkedPages?: string[];
	public readonly originalVersion?: SongApiContract;
	public readonly selectedLyricsId: number;
	public readonly selectedPvId: number;
	public readonly personalDescriptionAuthor?: ArtistApiContract;
	public readonly personalDescriptionText?: string;
	public readonly songType: SongType;
	public readonly tagUsages: TagUsageForApiContract[];
	public readonly userRating: string;
	public readonly version: number;

	public constructor(
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
