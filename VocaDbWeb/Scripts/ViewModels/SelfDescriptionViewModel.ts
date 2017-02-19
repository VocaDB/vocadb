
namespace vdb.viewModels {

	export class SelfDescriptionViewModel {

		constructor(author: dc.ArtistContract, text: string, private getArtists: (callback: (result: dc.ArtistContract[]) => void) => void,
			private saveFunc: ((vm: SelfDescriptionViewModel) => void)) {

			this.author = new BasicEntryLinkViewModel<dc.ArtistContract>(author, artistId => _.find(this.artists(), a => a.id === artistId));
			this.text = ko.observable(text);

		}

		public artists = ko.observableArray<dc.ArtistContract>();

		public author: BasicEntryLinkViewModel<dc.ArtistContract>;

		public beginEdit = () => {
			if (!this.artists().length) {
				this.getArtists(artists => {
					this.artists(artists);
				});
			}
			this.editing(true);
		}

		public cancelEdit = () => this.editing(false);

		public editing = ko.observable(false);

		public save = () => {
			this.saveFunc(this);
			this.editing(false);
		}

		public text: KnockoutObservable<string>;

	}

}