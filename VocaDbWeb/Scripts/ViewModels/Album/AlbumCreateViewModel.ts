/// <reference path="../../typings/knockout/knockout.d.ts" />

module vdb.viewModels {

	import dc = vdb.dataContracts;
	import rep = vdb.repositories;

    export class AlbumCreateViewModel {

		constructor(private albumRepo: rep.AlbumRepository, private artistRepo: rep.ArtistRepository) {
			
		}

		private addArtist = (artistId: number) => {

			if (artistId) {
				this.artistRepo.getOne(artistId, artist => this.artists.push(artist));
			}

		};

		public artists = ko.observableArray<dc.ArtistContract>([]);

		public artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams = {
			acceptSelection: this.addArtist
		};

		public checkDuplicates = () => {

			var term1 = this.nameOriginal();
			var term2 = this.nameRomaji();
			var term3 = this.nameEnglish();

			this.albumRepo.findDuplicate({ term1: term1, term2: term2, term3: term3 }, result => {

				this.dupeEntries(result);

			});

		};

		public dupeEntries = ko.observableArray<dc.DuplicateEntryResultContract>([]);

		public nameOriginal = ko.observable("");
		public nameRomaji = ko.observable("");
		public nameEnglish = ko.observable("");

		public removeArtist = (artist: dc.ArtistContract) => {
			this.artists.remove(artist);
		};

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);

    }

}