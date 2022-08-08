import ArtistForAlbumEditStore from '@/Stores/ArtistForAlbumEditStore';
import { action, makeObservable, observable } from 'mobx';

export default class CustomNameEditStore {
	@observable public artistLink?: ArtistForAlbumEditStore;
	@observable public dialogVisible = false;
	@observable public name = '';

	public constructor() {
		makeObservable(this);
	}

	@action public open = (artist: ArtistForAlbumEditStore): void => {
		this.artistLink = artist;
		this.name = artist.isCustomName ? artist.name : '';
		this.dialogVisible = true;
	};

	@action public save = (): void => {
		const isCustomName = !!this.name;

		this.artistLink!.isCustomName = isCustomName;
		this.dialogVisible = false;

		if (isCustomName) this.artistLink!.name = this.name!;
	};
}
