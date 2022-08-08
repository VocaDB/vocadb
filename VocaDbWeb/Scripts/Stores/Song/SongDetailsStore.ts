import LyricsForSongContract from '@/DataContracts/Song/LyricsForSongContract';
import SongApiContract from '@/DataContracts/Song/SongApiContract';
import { SongDetailsAjax } from '@/DataContracts/Song/SongDetailsForApi';
import SongListBaseContract from '@/DataContracts/SongListBaseContract';
import TagSelectionContract from '@/DataContracts/Tag/TagSelectionContract';
import RatedSongForUserForApiContract from '@/DataContracts/User/RatedSongForUserForApiContract';
import UserApiContract from '@/DataContracts/User/UserApiContract';
import ArtistHelper from '@/Helpers/ArtistHelper';
import EntryType from '@/Models/EntryType';
import LoginManager from '@/Models/LoginManager';
import SongVoteRating from '@/Models/SongVoteRating';
import SongType from '@/Models/Songs/SongType';
import ArtistRepository from '@/Repositories/ArtistRepository';
import SongRepository from '@/Repositories/SongRepository';
import UserRepository from '@/Repositories/UserRepository';
import GlobalValues from '@/Shared/GlobalValues';
import HttpClient from '@/Shared/HttpClient';
import EditableCommentsStore from '@/Stores/EditableCommentsStore';
import EnglishTranslatedStringStore from '@/Stores/Globalization/EnglishTranslatedStringStore';
import PVRatingButtonsStore from '@/Stores/PVRatingButtonsStore';
import ReportEntryStore from '@/Stores/ReportEntryStore';
import SelfDescriptionStore from '@/Stores/SelfDescriptionStore';
import SongLyricsStore from '@/Stores/Song/SongLyricsStore';
import TagListStore from '@/Stores/Tag/TagListStore';
import TagsEditStore from '@/Stores/Tag/TagsEditStore';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';

const fav = SongVoteRating[SongVoteRating.Favorite];
const like = SongVoteRating[SongVoteRating.Like];

export class RatingsStore {
	@observable public popupVisible = false;
	@observable public ratings: RatedSongForUserForApiContract[] = [];

	public constructor() {
		makeObservable(this);
	}

	@computed public get favorites(): UserApiContract[] {
		return _.chain(this.ratings)
			.filter((r) => !!r.user && r.rating === fav)
			.take(20)
			.map((r) => r.user!)
			.sortBy((u) => u.name)
			.value();
	}

	@computed public get favoritesCount(): number {
		return _.chain(this.ratings)
			.filter((r) => r.rating === fav)
			.size()
			.value();
	}

	@computed public get likes(): UserApiContract[] {
		return _.chain(this.ratings)
			.filter((r) => !!r.user && r.rating === like)
			.take(20)
			.map((r) => r.user!)
			.sortBy((u) => u.name)
			.value();
	}

	@computed public get likesCount(): number {
		return _.chain(this.ratings)
			.filter((r) => r.rating === like)
			.size()
			.value();
	}

	@computed public get hiddenRatingsCount(): number {
		return _.chain(this.ratings)
			.filter((r) => !r.user)
			.size()
			.value();
	}

	@computed public get showFavorites(): boolean {
		return !!this.favorites.length;
	}

	@computed public get showLikes(): boolean {
		return !!this.likes.length;
	}
}

interface SongLinkWithUrl {
	entry?: SongApiContract;
	url?: string;
	domain?: string;
}

export class SongInListsStore {
	@observable public contentHtml?: string;
	@observable public dialogVisible = false;

	public constructor(
		private readonly songRepo: SongRepository,
		private readonly songId: number,
	) {
		makeObservable(this);
	}

	public show = (): void => {
		this.songRepo.songListsForSong({ songId: this.songId }).then((result) =>
			runInAction(() => {
				this.contentHtml = result;
				this.dialogVisible = true;
			}),
		);
	};
}

export class SongListsStore {
	public static readonly tabName_Personal = 'Personal';
	public static readonly tabName_Featured = 'Featured';
	public static readonly tabName_New = 'New';

	@observable public dialogVisible = false;
	@observable public featuredLists: SongListBaseContract[] = [];
	@observable public newListName = '';
	@observable public notes = '';
	@observable public personalLists: SongListBaseContract[] = [];
	@observable public selectedListId?: number;
	@observable public tabName = SongListsStore.tabName_Personal;

	public constructor(
		private readonly songRepo: SongRepository,
		private readonly songId: number,
	) {
		makeObservable(this);
	}

	@computed public get isValid(): boolean {
		return !!this.selectedListId || this.newListName.length > 0;
	}

	@computed public get songLists(): SongListBaseContract[] {
		return this.tabName === SongListsStore.tabName_Personal
			? this.personalLists
			: this.featuredLists;
	}

	public addSongToList = (): Promise<void> => {
		if (!this.isValid) return Promise.reject();

		const listId =
			this.tabName !== SongListsStore.tabName_New
				? this.selectedListId || 0
				: 0;
		return this.songRepo
			.addSongToList({
				listId: listId,
				songId: this.songId,
				notes: this.notes,
				newListName: this.newListName,
			})
			.then(() => {
				runInAction(() => {
					this.notes = '';
					this.dialogVisible = false;
				});
			});
	};

	public showSongLists = (): void => {
		this.songRepo
			.songListsForUser({ ignoreSongId: this.songId })
			.then((songLists) => {
				const personalLists = songLists.filter(
					(list) => list.featuredCategory === 'Nothing',
				);
				const featuredLists = songLists.filter(
					(list) => list.featuredCategory !== 'Nothing',
				);

				runInAction(() => {
					this.personalLists = personalLists;
					this.featuredLists = featuredLists;

					if (personalLists.length)
						this.tabName = SongListsStore.tabName_Personal;
					else if (featuredLists.length)
						this.tabName = SongListsStore.tabName_Featured;
					else this.tabName = SongListsStore.tabName_New;

					this.newListName = '';
					this.selectedListId =
						this.songLists.length > 0 ? this.songLists[0].id : undefined;
					this.dialogVisible = true;
				});
			});
	};
}

// Store for the song details view.
export default class SongDetailsStore {
	@observable public allVersionsVisible = false;
	public readonly comments: EditableCommentsStore;
	public readonly id: number;
	@observable public maintenanceDialogVisible = false;
	@observable public originalVersion: SongLinkWithUrl;
	public readonly reportStore: ReportEntryStore;
	public readonly lyricsStore: SongLyricsStore;
	@observable public selectedPvId: number;
	public readonly personalDescription: SelfDescriptionStore;
	public readonly description: EnglishTranslatedStringStore;
	public readonly songInListsDialog: SongInListsStore;
	public readonly songListDialog: SongListsStore;
	public readonly tagsEditStore: TagsEditStore;
	public readonly tagUsages: TagListStore;
	public readonly ratingsDialogStore = new RatingsStore();
	public readonly userRating: PVRatingButtonsStore;

	public constructor(
		private readonly values: GlobalValues,
		loginManager: LoginManager,
		private readonly httpClient: HttpClient,
		private readonly songRepo: SongRepository,
		userRepo: UserRepository,
		artistRepo: ArtistRepository,
		showTranslatedDescription: boolean,
		data: SongDetailsAjax,
		canDeleteAllComments: boolean,
	) {
		makeObservable(this);

		this.id = data.id;

		this.userRating = new PVRatingButtonsStore(userRepo, {
			id: data.id,
			vote: data.userRating,
		});

		this.comments = new EditableCommentsStore(
			loginManager,
			songRepo,
			this.id,
			canDeleteAllComments,
			canDeleteAllComments,
			false,
			data.latestComments,
			true,
		);

		this.originalVersion = { entry: data.originalVersion };

		this.reportStore = new ReportEntryStore((reportType, notes) => {
			return songRepo.createReport({
				songId: this.id,
				reportType: reportType,
				notes: notes,
				versionNumber: undefined,
			});
		});

		this.personalDescription = new SelfDescriptionStore(
			values,
			data.personalDescriptionAuthor,
			data.personalDescriptionText,
			artistRepo,
			() =>
				songRepo
					.getOneWithComponents({
						id: this.id,
						fields: 'Artists',
						lang: values.languagePreference,
					})
					.then((result) => {
						const artists = _.chain(result.artists)
							.filter(ArtistHelper.isValidForPersonalDescription)
							.map((a) => a.artist)
							.value();
						return artists;
					}),
			(store) =>
				songRepo.updatePersonalDescription({
					songId: this.id,
					text: store.text,
					author: store.author.entry,
				}),
		);

		this.songInListsDialog = new SongInListsStore(songRepo, this.id);
		this.songListDialog = new SongListsStore(songRepo, this.id);
		this.lyricsStore = new SongLyricsStore(
			songRepo,
			data.selectedLyricsId,
			data.version,
		);
		this.selectedPvId = data.selectedPvId;

		this.description = new EnglishTranslatedStringStore(
			showTranslatedDescription,
		);

		this.tagsEditStore = new TagsEditStore(
			{
				getTagSelections: (): Promise<TagSelectionContract[]> =>
					userRepo.getSongTagSelections({ songId: this.id }),
				saveTagSelections: (tags): Promise<void> =>
					userRepo
						.updateSongTags({ songId: this.id, tags: tags })
						.then(this.tagUsages.updateTagUsages),
			},
			EntryType.Song,
			() => songRepo.getTagSuggestions({ songId: this.id }),
		);

		this.tagUsages = new TagListStore(data.tagUsages);

		if (data.songType !== SongType.Original && !this.originalVersion.entry) {
			this.getOriginal(data.linkedPages);
		}
	}

	@computed public get selectedLyrics(): LyricsForSongContract | undefined {
		return this.lyricsStore.selectedLyrics;
	}

	@computed public get selectedLyricsId(): number {
		return this.lyricsStore.selectedLyricsId;
	}
	public set selectedLyricsId(value: number) {
		this.lyricsStore.selectedLyricsId = value;
	}

	private getMatchedSite = (
		page: string,
	): { siteUrl: string; id: number } | undefined => {
		// http://utaitedb.net/S/1234 or http://utaitedb.net/Song/Details/1234
		const regex = /(http(?:s)?:\/\/(?:(?:utaitedb\.net)|(?:vocadb\.net)|(?:touhoudb\.com))\/)(?:(?:Song)\/Details|(?:S))\/(\d+)/g;
		const match = regex.exec(page);

		if (!match || match.length < 3) return undefined;

		const siteUrl = match[1].replace('http://', 'https://'); // either http://utaitedb.net/ or http://vocadb.net/
		const id = parseInt(match[2]);

		return { siteUrl: siteUrl, id: id };
	};

	private getOriginal = (linkedPages?: string[]): void => {
		if (!linkedPages || !linkedPages.length) return;

		const page = linkedPages[0];
		const match = this.getMatchedSite(page);

		if (!match) return;

		const { siteUrl, id } = match;

		const songRepo = new SongRepository(this.httpClient, siteUrl);

		// TODO: this should be cached, but first we need to make sure the other instances are not cached.
		songRepo
			.getOneWithComponents({
				id: id,
				fields: 'None',
				lang: this.values.languagePreference,
			})
			.then((song) => {
				if (song.songType === SongType.Original) {
					runInAction(() => {
						this.originalVersion = { entry: song, url: page, domain: siteUrl };
					});
				}
			});
	};

	public getUsers = (): void => {
		this.songRepo.getRatings({ songId: this.id }).then((result) =>
			runInAction(() => {
				this.ratingsDialogStore.ratings = result;
				this.ratingsDialogStore.popupVisible = true;
			}),
		);
	};

	@action public showAllVersions = (): void => {
		this.allVersionsVisible = true;
	};
}
