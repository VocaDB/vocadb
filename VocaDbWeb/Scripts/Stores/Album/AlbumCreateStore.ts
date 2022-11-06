import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import { DuplicateEntryResultContract } from '@/DataContracts/DuplicateEntryResultContract';
import { LocalizedStringContract } from '@/DataContracts/Globalization/LocalizedStringContract';
import { AlbumType } from '@/Models/Albums/AlbumType';
import { ContentLanguageSelection } from '@/Models/Globalization/ContentLanguageSelection';
import { AlbumRepository } from '@/Repositories/AlbumRepository';
import { ArtistRepository } from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { pull } from 'lodash-es';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class AlbumCreateStore {
	@observable artists: ArtistContract[] = [];
	@observable discType = AlbumType.Unknown;
	@observable dupeEntries: DuplicateEntryResultContract[] = [];
	@observable errors?: Record<string, string[]>;
	@observable nameOriginal = '';
	@observable nameRomaji = '';
	@observable nameEnglish = '';
	@observable submitting = false;

	constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	addArtist = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.artists.push(artist);
		});
	};

	checkDuplicates = (): void => {
		const term1 = this.nameOriginal;
		const term2 = this.nameRomaji;
		const term3 = this.nameEnglish;

		this.albumRepo
			.findDuplicate({
				params: { term1: term1, term2: term2, term3: term3 },
			})
			.then((result) =>
				runInAction(() => {
					this.dupeEntries = result;
				}),
			);
	};

	@action removeArtist = (artist: ArtistContract): void => {
		pull(this.artists, artist);
	};

	@action submit = async (requestToken: string): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.albumRepo.create(requestToken, {
				artists: this.artists,
				discType: this.discType,
				names: ([] as LocalizedStringContract[]).concat(
					this.nameOriginal
						? {
								language: ContentLanguageSelection.Japanese,
								value: this.nameOriginal,
						  }
						: [],
					this.nameRomaji
						? {
								language: ContentLanguageSelection.Romaji,
								value: this.nameRomaji,
						  }
						: [],
					this.nameEnglish
						? {
								language: ContentLanguageSelection.English,
								value: this.nameEnglish,
						  }
						: [],
				),
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
