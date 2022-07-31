import ArtistContract from '@DataContracts/Artist/ArtistContract';
import DuplicateEntryResultContract from '@DataContracts/DuplicateEntryResultContract';
import LocalizedStringContract from '@DataContracts/Globalization/LocalizedStringContract';
import AlbumType from '@Models/Albums/AlbumType';
import ContentLanguageSelection from '@Models/Globalization/ContentLanguageSelection';
import AlbumRepository from '@Repositories/AlbumRepository';
import ArtistRepository from '@Repositories/ArtistRepository';
import GlobalValues from '@Shared/GlobalValues';
import _ from 'lodash';
import { action, makeObservable, observable, runInAction } from 'mobx';

export default class AlbumCreateStore {
	@observable public artists: ArtistContract[] = [];
	@observable public discType = AlbumType.Unknown;
	@observable public dupeEntries: DuplicateEntryResultContract[] = [];
	@observable public errors?: Record<string, string[]>;
	@observable public nameOriginal = '';
	@observable public nameRomaji = '';
	@observable public nameEnglish = '';
	@observable public submitting = false;

	public constructor(
		private readonly values: GlobalValues,
		private readonly albumRepo: AlbumRepository,
		private readonly artistRepo: ArtistRepository,
	) {
		makeObservable(this);
	}

	public addArtist = async (artistId?: number): Promise<void> => {
		if (!artistId) return;

		const artist = await this.artistRepo.getOne({
			id: artistId,
			lang: this.values.languagePreference,
		});

		runInAction(() => {
			this.artists.push(artist);
		});
	};

	public checkDuplicates = (): void => {
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

	@action public removeArtist = (artist: ArtistContract): void => {
		_.pull(this.artists, artist);
	};

	@action public submit = async (): Promise<number> => {
		this.submitting = true;

		try {
			const id = await this.albumRepo.create({
				artists: this.artists,
				discType: this.discType,
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
