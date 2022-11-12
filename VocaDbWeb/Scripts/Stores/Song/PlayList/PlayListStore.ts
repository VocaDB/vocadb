import { PagingProperties } from '@/DataContracts/PagingPropertiesContract';
import { PartialFindResultContract } from '@/DataContracts/PartialFindResultContract';
import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { DateTimeHelper } from '@/Helpers/DateTimeHelper';
import { PVHelper } from '@/Helpers/PVHelper';
import { ContentLanguagePreference } from '@/Models/Globalization/ContentLanguagePreference';
import { PVServiceIcons } from '@/Models/PVServiceIcons';
import { PVService } from '@/Models/PVs/PVService';
import { SongOptionalField } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { UrlMapper } from '@/Shared/UrlMapper';
import { IPVPlayerSong, PVPlayerStore } from '@/Stores/PVs/PVPlayerStore';
import { ServerSidePagingStore } from '@/Stores/ServerSidePagingStore';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export interface ISongForPlayList {
	// Song index in playlist with current filters, starting from 0.
	// In shuffle mode songs may be loaded out of order.
	indexInPlayList: number;

	name: string;

	song: SongApiContract;
}

export interface IPlayListRepository {
	getSongs(
		pvServices: PVService[],
		paging: PagingProperties,
		fields: SongOptionalField[],
		lang: ContentLanguagePreference,
	): Promise<PartialFindResultContract<ISongForPlayList>>;
}

export class PlayListStore {
	isInit = false;
	@observable loading = true; // Currently loading for data
	@observable page: ISongForPlayList[] = []; // Current page of items
	readonly paging = new ServerSidePagingStore(30); // Paging view model
	pvServiceIcons: PVServiceIcons;

	constructor(
		private readonly values: GlobalValues,
		urlMapper: UrlMapper,
		private readonly songListRepo: IPlayListRepository,
		private readonly pvPlayerStore: PVPlayerStore,
	) {
		makeObservable(this);

		pvPlayerStore.nextSong = this.nextSong;
		pvPlayerStore.resetSong = (): void => {
			runInAction(() => {
				pvPlayerStore.selectedSong = this.page.find((song) =>
					pvPlayerStore.songIsValid(song),
				);
			});
		};

		reaction(
			() => pvPlayerStore.autoplay,
			() => {
				this.updateResultsWithTotalCount();
			},
		);

		this.pvServiceIcons = new PVServiceIcons(urlMapper);
	}

	@computed private get hasMoreSongs(): boolean {
		return this.page.length < this.paging.totalItems;
	}

	@computed get songsLoaded(): number {
		return this.page.length;
	}

	formatLength = (length: number): string =>
		DateTimeHelper.formatFromSeconds(length);

	private getRandomSongIndex = (): number => {
		return Math.floor(Math.random() * this.paging.totalItems);
	};

	// Gets the index of the currently playing song.
	// -1 if the currently playing song isn't in the current list of songs, which is possible if the search filters were changed.
	private getSongIndex = (song: IPVPlayerSong): number => {
		// Might need to build a lookup for this for large playlists
		for (var i = 0; i < this.page.length; ++i) {
			if (this.page[i].song.id === song.song.id) return i;
		}

		return -1;
	};

	// Gets a song with a specific playlist index.
	// If shuffle is enabled, this index is NOT the same as the song index in the list of songs.
	private getSongWithPlayListIndex = (
		index: number,
	): ISongForPlayList | undefined => {
		// Might need to build a lookup for this for large playlists
		return this.page.find((s) => s.indexInPlayList === index);
	};

	@action private playSong = (song?: ISongForPlayList): void => {
		this.pvPlayerStore.selectedSong = song;
	};

	scrollEnd = (): void => {
		// For now, disable autoload in shuffle mode
		if (this.hasMoreSongs && !this.pvPlayerStore.shuffle) {
			this.paging.nextPage();
			this.updateResultsWithoutTotalCount();
		}
	};

	private pauseNotifications = false;

	@action updateResults = async (
		clearResults: boolean = true,
		songWithIndex?: number,
	): Promise<void> => {
		// Disable duplicate updates
		if (this.pauseNotifications) return;

		this.pauseNotifications = true;
		this.loading = true;

		if (clearResults) {
			this.page = [];
			this.paging.goToFirstPage();
		}

		const pagingProperties = this.paging.getPagingProperties(clearResults);

		if (songWithIndex) {
			pagingProperties.start = songWithIndex;
			pagingProperties.maxEntries = 1;
		}

		const services = this.pvPlayerStore.autoplay
			? PVPlayerStore.autoplayPVServices
			: [
					PVService.Youtube,
					PVService.SoundCloud,
					PVService.NicoNicoDouga,
					PVService.Bilibili,
					PVService.Vimeo,
					PVService.Piapro,
					PVService.File,
					PVService.LocalFile,
			  ];

		const result = await this.songListRepo.getSongs(
			services,
			pagingProperties,
			[SongOptionalField.AdditionalNames, SongOptionalField.MainPicture],
			this.values.languagePreference,
		);

		this.pauseNotifications = false;

		runInAction(() => {
			if (pagingProperties.getTotalCount)
				this.paging.totalItems = result.totalCount;

			for (const item of result.items) {
				item.song.pvServicesArray = PVHelper.pvServicesArrayFromString(
					item.song.pvServices,
				);
				this.page.push(item);
			}

			this.loading = false;
		});

		if (
			result.items &&
			result.items.length &&
			!this.pvPlayerStore.selectedSong
		) {
			const song = this.pvPlayerStore.shuffle
				? result.items[Math.floor(Math.random() * result.items.length)]
				: result.items[0];
			this.playSong(song);
		}
	};

	updateResultsWithTotalCount = (): Promise<void> =>
		this.updateResults(true, undefined);

	updateResultsWithoutTotalCount = (): Promise<void> =>
		this.updateResults(false);

	nextSong = (): void => {
		if (this.paging.totalItems === 0) return;

		if (this.pvPlayerStore.shuffle) {
			// Get a random index
			const index = this.getRandomSongIndex();

			// Check if song is already loaded
			const song = this.getSongWithPlayListIndex(index);

			if (song) {
				this.playSong(song);
			} else {
				// Song not loaded, load that one song
				this.updateResults(false, index).then(() => {
					this.playSong(this.getSongWithPlayListIndex(index));
				});
			}
		} else {
			// Get the index of the next song to be played
			const index = this.getSongIndex(this.pvPlayerStore.selectedSong!) + 1;

			if (index < this.songsLoaded) {
				this.playSong(this.page[index]);
			} else {
				if (this.hasMoreSongs) {
					this.paging.nextPage();
					this.updateResults(false, undefined).then(() => {
						this.playSong(this.page[index]);
					});
				} else {
					this.playSong(this.page[0]);
				}
			}
		}
	};
}
