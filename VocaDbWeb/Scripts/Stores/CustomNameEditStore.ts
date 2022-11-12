import { ArtistForAlbumEditStore } from '@/Stores/ArtistForAlbumEditStore';
import { action, makeObservable, observable } from 'mobx';

export class CustomNameEditStore {
	@observable artistLink?: ArtistForAlbumEditStore;
	@observable dialogVisible = false;
	@observable name = '';

	constructor() {
		makeObservable(this);
	}

	@action open = (artist: ArtistForAlbumEditStore): void => {
		this.artistLink = artist;
		this.name = artist.isCustomName ? artist.name : '';
		this.dialogVisible = true;
	};

	@action save = (): void => {
		const isCustomName = !!this.name;

		this.artistLink!.isCustomName = isCustomName;
		this.dialogVisible = false;

		if (isCustomName) this.artistLink!.name = this.name!;
	};
}
