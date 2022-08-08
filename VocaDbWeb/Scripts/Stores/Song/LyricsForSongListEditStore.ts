import LyricsForSongContract from '@/DataContracts/Song/LyricsForSongContract';
import ContentLanguageSelection from '@/Models/Globalization/ContentLanguageSelection';
import TranslationType from '@/Models/Globalization/TranslationType';
import _ from 'lodash';
import { action, computed, makeObservable, observable } from 'mobx';

import BasicListEditStore from '../BasicListEditStore';

export class LyricsForSongEditStore {
	@observable public cultureCode: string;
	@observable public id: number;
	public readonly isNew: boolean;
	@observable public language: string /* TODO: enum */;
	@observable public source: string;
	@observable public translationType: string /* TODO: enum */;
	@observable public url: string;
	@observable public value: string;

	public constructor(contract?: LyricsForSongContract) {
		makeObservable(this);

		if (contract) {
			this.id = contract.id!;
			this.cultureCode = contract.cultureCode!;
			this.language = contract.language!;
			this.source = contract.source!;
			this.translationType = contract.translationType;
			this.url = contract.url!;
			this.value = contract.value!;
		} else {
			this.id = 0;
			this.cultureCode = '';
			this.language =
				ContentLanguageSelection[ContentLanguageSelection.Unspecified];
			this.source = '';
			this.translationType = TranslationType[TranslationType.Translation];
			this.url = '';
			this.value = '';
		}

		this.isNew = !contract;
	}

	@computed public get showLanguageSelection(): boolean {
		return this.translationType !== TranslationType[TranslationType.Romanized];
	}
}

export default class LyricsForSongListEditStore extends BasicListEditStore<
	LyricsForSongEditStore,
	LyricsForSongContract
> {
	public readonly original: LyricsForSongEditStore;
	public readonly romanized: LyricsForSongEditStore;

	private find = (translationType: string): LyricsForSongEditStore => {
		let store = this.items.find((i) => i.translationType === translationType);
		if (store) _.pull(this.items, store);
		else {
			store = new LyricsForSongEditStore({ translationType: translationType });
		}
		return store;
	};

	public constructor(contracts: LyricsForSongContract[]) {
		super(LyricsForSongEditStore, contracts);

		this.original = this.find('Original');
		this.romanized = this.find('Romanized');
	}

	@action public changeToOriginal = (lyrics: LyricsForSongEditStore): void => {
		this.original.id = lyrics.id;
		this.original.value = lyrics.value;
		this.original.cultureCode = lyrics.cultureCode;
		this.original.source = lyrics.source;
		this.original.url = lyrics.url;
		_.pull(this.items, lyrics);
	};

	@action public changeToTranslation = (
		lyrics: LyricsForSongEditStore,
	): void => {
		if (lyrics === this.original) {
			const newLyrics = new LyricsForSongEditStore({
				id: this.original.id,
				cultureCode: this.original.cultureCode,
				source: this.original.source,
				url: this.original.url,
				value: this.original.value,
				translationType: TranslationType[TranslationType.Translation],
			});

			this.items.push(newLyrics);

			this.original.id = 0;
			this.original.value = '';
			this.original.cultureCode = '';
			this.original.source = '';
			this.original.url = '';
		} else {
			lyrics.translationType = TranslationType[TranslationType.Translation];
		}
	};

	public toContracts = (): LyricsForSongContract[] => {
		return _.chain([this.original, this.romanized])
			.concat(this.items)
			.filter((i) => !!i.value)
			.value();
	};
}
