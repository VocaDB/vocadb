import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongContract } from '@/DataContracts/Song/SongContract';
import { SongInListEditContract } from '@/DataContracts/Song/SongInListEditContract';
import { SongListForEditContract } from '@/DataContracts/Song/SongListForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { AntiforgeryRepository } from '@/Repositories/AntiforgeryRepository';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import dayjs from 'dayjs';
import { pull } from 'lodash-es';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

export class SongInListEditStore {
	private static nextId = 1;

	readonly id: number;
	@observable notes: string;
	@observable order: number;
	readonly song: SongApiContract;
	readonly songInListId: number;

	constructor(data: SongInListEditContract) {
		makeObservable(this);

		this.id = SongInListEditStore.nextId++;
		this.songInListId = data.songInListId;
		this.notes = data.notes;
		this.order = data.order;
		this.song = data.song;
	}
}

export interface CsvData {
	order: number;
	id: number;
	notes: string;
}

export class CsvDataStore {
	readonly id: number;
	@observable notes: string;
	@observable order: number;

	constructor(data: CsvData) {
		makeObservable(this);

		this.id = data.id;
		this.notes = data.notes;
		this.order = data.order;
	}
}

export class SongListEditStore {
	readonly deleteStore: DeleteEntryStore;
	@observable description: string;
	@observable errors?: Record<string, string[]>;
	@observable eventDateDate?: Date;
	@observable featuredCategory: SongListFeaturedCategory;
	@observable name: string;
	@observable songLinks: SongInListEditStore[];
	@observable csvData: CsvData[] | null = null;
	@observable status: EntryStatus;
	@observable submitting = false;
	readonly trashStore: DeleteEntryStore;
	@observable updateNotes = '';

	constructor(
		private readonly values: GlobalValues,
		antiforgeryRepo: AntiforgeryRepository,
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		readonly contract: SongListForEditContract,
	) {
		makeObservable(this);

		this.deleteStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				this.songListRepo.delete(requestToken, {
					id: this.contract.id,
					notes: notes,
					hardDelete: false,
				}),
		);

		this.trashStore = new DeleteEntryStore(
			antiforgeryRepo,
			(requestToken, notes) =>
				this.songListRepo.delete(requestToken, {
					id: this.contract.id,
					notes: notes,
					hardDelete: true,
				}),
		);

		this.songLinks = [];

		this.name = contract.name;
		this.description = contract.description;
		this.eventDateDate = contract.eventDate
			? dayjs(contract.eventDate).toDate()
			: undefined; // Assume server date is UTC
		this.featuredCategory = contract.featuredCategory;
		this.status = contract.status;

		const mappedSongs = contract.songLinks.map(
			(item) => new SongInListEditStore(item),
		);
		this.songLinks = mappedSongs;

		reaction(
			() => this.songLinks.map((songLink) => ({ order: songLink.order })),
			(songLinks) => {
				for (let track = 0; track < songLinks.length; ++track) {
					this.songLinks[track].order = track + 1;
				}
			},
		);
	}

	@computed get eventDate(): string | undefined {
		return this.eventDateDate ? this.eventDateDate.toISOString() : undefined;
	}

	importCsvData = (data: CsvData[]): void => {
		this.csvData = data;
	};

	acceptSongSelection = (songId?: number): void => {
		if (!songId) return;

		this.songRepo
			.getOne({ id: songId, lang: this.values.languagePreference })
			.then((song) => {
				runInAction(() => {
					const songInList = new SongInListEditStore({
						songInListId: 0,
						order: 0,
						notes: '',
						song: song,
					});
					this.songLinks.push(songInList);
				});
			});
	};

	@action removeSong = (songLink: SongInListEditStore): void => {
		pull(this.songLinks, songLink);
	};

	@action submit = async (
		requestToken: string,
		thumbPicUpload: File | undefined,
	): Promise<number> => {
		this.submitting = true;

		const songLinks = this.csvData
			? this.csvData.map(
					(d) =>
						new SongInListEditStore({
							songInListId: 0,
							order: d.order,
							notes: d.notes,
							song: { id: d.id } as SongContract,
						}),
			  )
			: this.songLinks;

		try {
			const id = await this.songListRepo.edit(
				requestToken,
				{
					author: undefined!,
					description: this.description,
					eventDate: this.eventDate,
					featuredCategory: this.featuredCategory,
					id: this.contract.id,
					name: this.name,
					songLinks,
					status: this.status,
					updateNotes: this.updateNotes,
				},
				thumbPicUpload,
			);

			return id;
		} catch (error: any) {
			if (error.response) {
				runInAction(() => {
					this.errors = undefined;

					if (error.response.status === 400)
						this.errors = error.response.data.errors;
				});
			}

			throw error;
		} finally {
			runInAction(() => {
				this.submitting = false;
			});
		}
	};
}
