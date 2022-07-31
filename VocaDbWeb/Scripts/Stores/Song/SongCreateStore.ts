import ArtistContract from '@DataContracts/Artist/ArtistContract';
import DuplicateEntryResultContract from '@DataContracts/DuplicateEntryResultContract';
import LocalizedStringContract from '@DataContracts/Globalization/LocalizedStringContract';
import ArtistForSongContract from '@DataContracts/Song/ArtistForSongContract';
import SongContract from '@DataContracts/Song/SongContract';
import TagApiContract from '@DataContracts/Tag/TagApiContract';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import SongType from '@Models/Songs/SongType';
import ArtistRepository from '@Repositories/ArtistRepository';
import SongRepository from '@Repositories/SongRepository';
import TagRepository from '@Repositories/TagRepository';
import EntryUrlMapper from '@Shared/EntryUrlMapper';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import {
	action,
	computed,
	makeObservable,
	observable,
	reaction,
	runInAction,
} from 'mobx';

import EntryType from '../../Models/EntryType';
import BasicEntryLinkStore from '../BasicEntryLinkStore';

export default class SongCreateStore {
	@observable public artists: ArtistContract[] = [];
	@observable public draft = false;
	@observable public dupeEntries: DuplicateEntryResultContract[] = [];
	@observable public errors?: Record<string, string[]>;
	@observable public nameOriginal = '';
	@observable public nameRomaji = '';
	@observable public nameEnglish = '';
	public readonly originalVersion: BasicEntryLinkStore<SongContract>;
	@observable public pv1 = '';
	@observable public pv2 = '';
	@observable public songType = SongType.Original;
	@observable public songTypeTag?: TagApiContract;
	@observable public submitting = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly songRepo: SongRepository,
		private readonly artistRepo: ArtistRepository,
		private readonly tagRepo: TagRepository,
		data?: {
			nameOriginal?: string;
			nameRomaji?: string;
			nameEnglish?: string;
			pvUrl?: string;
			reprintPVUrl?: string;
			artists?: ArtistContract[];
		},
	) {
		makeObservable(this);

		this.originalVersion = new BasicEntryLinkStore<SongContract>((entryId) =>
			songRepo.getOne({
				id: entryId,
				lang: values.languagePreference,
			}),
		);

		if (data) {
			this.nameOriginal = data.nameOriginal || '';
			this.nameRomaji = data.nameRomaji || '';
			this.nameEnglish = data.nameEnglish || '';
			this.pv1 = data.pvUrl || '';
			this.pv2 = data.reprintPVUrl || '';
			this.artists = data.artists || [];
		}

		if (this.pv1) this.checkDuplicatesAndPV();

		reaction(() => this.songType, this.getSongTypeTag);
		this.getSongTypeTag(this.songType);
	}

	@computed public get canHaveOriginalVersion(): boolean {
		return this.songType !== SongType.Original;
	}

	@computed public get hasName(): boolean {
		return (
			this.nameOriginal.length > 0 ||
			this.nameRomaji.length > 0 ||
			this.nameEnglish.length > 0
		);
	}

	@computed public get isDuplicatePV(): boolean {
		return this.dupeEntries.some((item) => item.matchProperty === 'PV');
	}

	@computed
	public get originalSongSuggestions(): DuplicateEntryResultContract[] {
		if (!this.dupeEntries || this.dupeEntries.length === 0) return [];

		return _.take(this.dupeEntries, 3);
	}

	@computed public get songTypeName(): string | undefined {
		return this.songTypeTag?.name;
	}

	@computed public get songTypeInfo(): string | undefined {
		return this.songTypeTag?.description;
	}

	@computed public get songTypeTagUrl(): string | undefined {
		return EntryUrlMapper.details_tag_contract(this.songTypeTag);
	}

	private getSongTypeTag = async (songType: SongType): Promise<void> => {
		const tag = await this.tagRepo.getEntryTypeTag({
			entryType: EntryType.Song,
			subType: songType,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.songTypeTag = tag;
		});
	};

	private getArtistIds = (): number[] => {
		return this.artists.map((artist) => artist.id);
	};

	private _checkDuplicates = async (getPVInfo = false): Promise<void> => {
		const term1 = this.nameOriginal;
		const term2 = this.nameRomaji;
		const term3 = this.nameEnglish;
		const pv1 = this.pv1;
		const pv2 = this.pv2;
		const artists = this.getArtistIds();

		const result = await this.songRepo.findDuplicate({
			params: {
				term: [term1, term2, term3],
				pv: [pv1, pv2],
				artistIds: artists,
				getPVInfo: getPVInfo,
			},
		});

		runInAction(() => {
			this.dupeEntries = result.matches;

			if (result.title && !this.hasName) {
				if (result.titleLanguage === 'English') this.nameEnglish = result.title;
				else this.nameOriginal = result.title;
			}

			if (
				result.songType &&
				result.songType !== 'Unspecified' &&
				this.songType === SongType.Original
			) {
				this.songType = result.songType;
			}

			if (result.artists && this.artists.length === 0) {
				for (const artist of result.artists) this.artists.push(artist);
			}
		});
	};

	public checkDuplicates = (): Promise<void> => {
		return this._checkDuplicates(false);
	};

	public checkDuplicatesAndPV = (): Promise<void> => {
		return this._checkDuplicates(true);
	};

	public addArtist = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.artists.push(artist);
		});

		await this.checkDuplicates();
	};

	@action public removeArtist = (artist: ArtistContract): void => {
		_.pull(this.artists, artist);
	};

	public selectOriginal = async (
		dupe: DuplicateEntryResultContract,
	): Promise<void> => {
		const song = await this.songRepo.getOne({
			id: dupe.entry.id,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.originalVersion.id = song.id;
		});
	};

	@action public submit = async (): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.songRepo.create({
				artists: this.artists.map(
					(artist) =>
						({
							artist: artist,
						} as ArtistForSongContract),
				),
				draft: this.draft,
				names: ([] as LocalizedStringContract[]).concat(
					this.nameOriginal
						? {
								language:
									ContentLanguageSelection[ContentLanguageSelection.Japanese],
								value: this.nameOriginal,
						  }
						: [],
					this.nameRomaji
						? {
								language:
									ContentLanguageSelection[ContentLanguageSelection.Romaji],
								value: this.nameRomaji,
						  }
						: [],
					this.nameEnglish
						? {
								language:
									ContentLanguageSelection[ContentLanguageSelection.English],
								value: this.nameEnglish,
						  }
						: [],
				),
				originalVersion: this.originalVersion.entry,
				pvUrls: [this.pv1],
				reprintPVUrl: this.pv2,
				songType: this.songType,
			});

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
