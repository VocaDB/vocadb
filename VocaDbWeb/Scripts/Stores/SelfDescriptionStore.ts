import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArtistContract } from '@/DataContracts/Artist/ArtistContract';
import {
	ArtistOptionalField,
	ArtistRepository,
} from '@/Repositories/ArtistRepository';
import { GlobalValues } from '@/Shared/GlobalValues';
import { BasicEntryLinkStore } from '@/Stores/BasicEntryLinkStore';
import { action, makeObservable, observable, runInAction } from 'mobx';

export class SelfDescriptionStore {
	@observable public artists: ArtistContract[] = [];
	public readonly author: BasicEntryLinkStore<ArtistApiContract>;
	@observable public editing = false;
	private originalAuthor?: number;
	private originalText?: string;
	@observable public text?: string;

	public constructor(
		values: GlobalValues,
		author: ArtistApiContract | undefined,
		text: string | undefined,
		artistRepo: ArtistRepository,
		private readonly getArtists: () => Promise<ArtistContract[]>,
		private readonly saveFunc: (store: SelfDescriptionStore) => void,
	) {
		makeObservable(this);

		this.author = new BasicEntryLinkStore<ArtistApiContract>((artistId) =>
			artistRepo.getOneWithComponents({
				id: artistId,
				fields: [ArtistOptionalField.MainPicture],
				lang: values.languagePreference,
			}),
		);
		this.author.id = author?.id;
		this.text = text;
	}

	@action public beginEdit = (): void => {
		this.originalAuthor = this.author.id;
		this.originalText = this.text;

		if (this.artists.length) {
			this.editing = true;
		} else {
			this.getArtists().then((artists) =>
				runInAction(() => {
					this.artists = artists;
					this.editing = true;
				}),
			);
		}
	};

	@action public cancelEdit = (): void => {
		this.text = this.originalText;
		this.author.id = this.originalAuthor;
		this.editing = false;
	};

	@action public save = (): void => {
		this.saveFunc(this);
		this.editing = false;
	};
}
