import { SongApiContract } from '@/DataContracts/Song/SongApiContract';
import { SongInListEditContract } from '@/DataContracts/Song/SongInListEditContract';
import { SongListForEditContract } from '@/DataContracts/Song/SongListForEditContract';
import { EntryStatus } from '@/Models/EntryStatus';
import { SongListFeaturedCategory } from '@/Models/SongLists/SongListFeaturedCategory';
import { SongListRepository } from '@/Repositories/SongListRepository';
import { SongRepository } from '@/Repositories/SongRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { DeleteEntryStore } from '@/Stores/DeleteEntryStore';
import { pull } from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';
import moment from 'moment';

export class SongInListEditStore {
	private static nextId = 1;

	public readonly id: number;
	@observable public notes: string;
	@observable public order: number;
	public readonly song: SongApiContract;
	public readonly songInListId: number;

	public constructor(data: SongInListEditContract) {
		makeObservable(this);

		this.id = SongInListEditStore.nextId++;
		this.songInListId = data.songInListId;
		this.notes = data.notes;
		this.order = data.order;
		this.song = data.song;
	}
}

export class SongListEditStore {
	public readonly deleteStore = new DeleteEntryStore((notes) =>
		this.songListRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: false,
		}),
	);
	@observable public description: string;
	@observable public errors?: Record<string, string[]>;
	@observable public eventDateDate?: Date;
	@observable public featuredCategory: SongListFeaturedCategory;
	@observable public name: string;
	@observable public songLinks: SongInListEditStore[];
	@observable public status: EntryStatus;
	@observable public submitting = false;
	public readonly trashStore = new DeleteEntryStore((notes) =>
		this.songListRepo.delete({
			id: this.contract.id,
			notes: notes,
			hardDelete: true,
		}),
	);
	@observable public updateNotes = '';

	public constructor(
		private readonly values: GlobalValues,
		private readonly songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		public readonly contract: SongListForEditContract,
	) {
		makeObservable(this);

		this.songLinks = [];

		this.name = contract.name;
		this.description = contract.description;
		this.eventDateDate = contract.eventDate
			? moment(contract.eventDate).toDate()
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

	@computed public get eventDate(): string | undefined {
		return this.eventDateDate ? this.eventDateDate.toISOString() : undefined;
	}

	public acceptSongSelection = (songId?: number): void => {
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

	@action public removeSong = (songLink: SongInListEditStore): void => {
		pull(this.songLinks, songLink);
	};

	@action public submit = async (
		requestToken: string,
		thumbPicUpload: File | undefined,
	): Promise<number> => {
		this.submitting = true;

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
					songLinks: this.songLinks,
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
