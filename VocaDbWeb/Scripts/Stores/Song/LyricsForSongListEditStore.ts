import { LyricsForSongContract } from '@/DataContracts/Song/LyricsForSongContract';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { TranslationType } from '@/Models/Globalization/TranslationType';
import { BasicListEditStore } from '@/Stores/BasicListEditStore';
import { WebLinkMatcher } from '@vocadb/web-link-matcher';
import { pull } from 'lodash-es';
import { action, computed, makeObservable, observable, reaction } from 'mobx';

export class LyricsForSongEditStore {
	@observable cultureCode: string;
	@observable id: number;
	readonly isNew: boolean;
	@observable language: ContentLanguageSelection;
	@observable source: string;
	@observable translationType: string /* TODO: enum */;
	@observable url: string;
	@observable value: string;

	constructor(contract?: LyricsForSongContract) {
		makeObservable(this);

		if (contract) {
			this.id = contract.id!;
			this.cultureCode = contract.cultureCode!;
			this.language = contract.language!;
			this.source = contract.source;
			this.translationType = contract.translationType;
			this.url = contract.url;
			this.value = contract.value!;
		} else {
			this.id = 0;
			this.cultureCode = '';
			this.language = ContentLanguageSelection.Unspecified;
			this.source = '';
			this.translationType = TranslationType[TranslationType.Translation];
			this.url = '';
			this.value = '';
		}

		reaction(
			() => this.url,
			(url) => {
				if (this.source) return;

				const matcher = WebLinkMatcher.matchWebLink(url);

				if (matcher) this.source = matcher.desc;
			},
		);

		this.isNew = !contract;
	}

	@computed get showLanguageSelection(): boolean {
		return this.translationType !== TranslationType[TranslationType.Romanized];
	}
}

export class LyricsForSongListEditStore extends BasicListEditStore<
	LyricsForSongEditStore,
	LyricsForSongContract
> {
	readonly original: LyricsForSongEditStore;
	readonly romanized: LyricsForSongEditStore;

	private find = (translationType: string): LyricsForSongEditStore => {
		let store = this.items.find((i) => i.translationType === translationType);
		if (store) pull(this.items, store);
		else {
			store = new LyricsForSongEditStore({
				source: '',
				translationType: translationType,
				url: '',
			});
		}
		return store;
	};

	constructor(contracts: LyricsForSongContract[]) {
		super(LyricsForSongEditStore, contracts);

		this.original = this.find('Original');
		this.romanized = this.find('Romanized');
	}

	@action changeToOriginal = (lyrics: LyricsForSongEditStore): void => {
		this.original.id = lyrics.id;
		this.original.value = lyrics.value;
		this.original.cultureCode = lyrics.cultureCode;
		this.original.source = lyrics.source;
		this.original.url = lyrics.url;
		pull(this.items, lyrics);
	};

	@action changeToTranslation = (lyrics: LyricsForSongEditStore): void => {
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

	toContracts = (): LyricsForSongContract[] => {
		return [this.original, this.romanized]
			.concat(this.items)
			.filter((i) => !!i.value);
	};
}
