import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	runInAction,
} from 'mobx';
import moment from 'moment';

import SongApiContract from '../../DataContracts/Song/SongApiContract';
import SongInListEditContract from '../../DataContracts/Song/SongInListEditContract';
import SongListRepository from '../../Repositories/SongListRepository';
import SongRepository from '../../Repositories/SongRepository';
import GlobalValues from '../../Shared/GlobalValues';

export class SongInListEditStore {
	@observable public notes: string;
	@observable public order: number;
	public readonly song: SongApiContract;
	public readonly songInListId: number;

	public constructor(data: SongInListEditContract) {
		makeObservable(this);

		this.songInListId = data.songInListId;
		this.notes = data.notes;
		this.order = data.order;
		this.song = data.song;
	}
}

export default class SongListEditStore {
	public readonly currentName: string;
	@observable public description: string;
	@observable public eventDateDate?: Date;
	@observable public songLinks: SongInListEditStore[];
	@observable public featuredCategory: string /* TODO: enum */;
	public readonly id: number;
	@observable public name: string;
	@observable public status: string /* TODO: enum */;
	@observable public submitting = false;
	@observable public updateNotes = '';

	public constructor(
		private readonly values: GlobalValues,
		songListRepo: SongListRepository,
		private readonly songRepo: SongRepository,
		id: number,
	) {
		makeObservable(this);

		this.id = id;
		this.songLinks = [];

		if (this.id) {
			songListRepo.getForEdit({ id: id }).then((data) => {
				runInAction(() => {
					this.currentName = data.name;
					this.name = data.name;
					this.description = data.description;
					this.eventDateDate = data.eventDate
						? moment(data.eventDate).toDate()
						: undefined; // Assume server date is UTC
					this.featuredCategory = data.featuredCategory;
					this.status = data.status;

					const mappedSongs = _.map(
						data.songLinks,
						(item) => new SongInListEditStore(item),
					);
					this.songLinks = mappedSongs;
					// TODO
				});
			});
		} else {
			this.name = '';
			this.description = '';
			this.featuredCategory = 'Nothing' /* TODO: enum */;
			this.status = 'Draft' /* TODO: enum */;
			// TODO
		}

		// TODO
	}

	@computed public get eventDate(): string | undefined {
		return this.eventDateDate ? this.eventDateDate.toISOString() : undefined;
	}

	private acceptSongSelection = (songId?: number): void => {
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
		_.pull(this.songLinks, songLink);
	};

	@action public submit = (): boolean => {
		this.submitting = true;
		return true;
	};
}
