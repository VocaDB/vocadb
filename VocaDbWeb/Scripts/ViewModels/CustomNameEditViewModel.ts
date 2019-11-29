import ArtistForAlbumEditViewModel from './ArtistForAlbumEditViewModel';

//namespace vdb.viewModels {

	export default class CustomNameEditViewModel {

		public artistLink = ko.observable<ArtistForAlbumEditViewModel>();
		public dialogVisible = ko.observable(false);
		public name = ko.observable<string>(null);

		public open = (artist: ArtistForAlbumEditViewModel) => {

			this.artistLink(artist);
			this.name(artist.isCustomName ? artist.name() : "");
			this.dialogVisible(true);

		}

		public save = () => {

			const isCustomName = !!this.name();

			this.artistLink().isCustomName = isCustomName;
			this.dialogVisible(false);

			if (isCustomName) {
				this.artistLink().name(this.name());
			}

		}

	}

//}