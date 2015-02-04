
module vdb.viewModels {

    import dc = vdb.dataContracts;

    // View model for song creation view
    export class SongCreateViewModel {
        
        addArtist: (artistId: number) => void;

        artistSearchParams: vdb.knockoutExtensions.ArtistAutoCompleteParams;
        
        artists = ko.observableArray<dc.ArtistContract>([]);

		private getArtistIds = () => {
			return _.map(this.artists(), a => a.id);
		}

		public checkDuplicatesAndPV = () => {
			this.checkDuplicates(null, true);
		}

        public checkDuplicates = (event?: JQueryEventObject, getPVInfo = false) => {
	   
			var term1 = this.nameOriginal();
			var term2 = this.nameRomaji();
			var term3 = this.nameEnglish();
			var pv1 = this.pv1();
			var pv2 = this.pv2();
			var artists = this.getArtistIds();

			this.songRepository.findDuplicate(
				{ term1: term1, term2: term2, term3: term3, pv1: pv1, pv2: pv2, artistIds: artists, getPVInfo: getPVInfo },
				result => {

                this.dupeEntries(result.matches);

				if (result.title && !this.hasName()) {

					if (result.titleLanguage === "English") {
						this.nameEnglish(result.title);
					} else {
						this.nameOriginal(result.title);
					}

                }

                if (result.songType && result.songType !== "Unspecified" && this.songType() === "Original") {
                    this.songType(result.songType);
                }

                if (result.artists && this.artists().length === 0) {

                    _.forEach(result.artists, artist => {
                        this.artists.push(artist);
                    });

                }

            });
			 
		}

        dupeEntries = ko.observableArray<dc.DuplicateEntryResultContract>([]);

        isDuplicatePV: KnockoutComputed<boolean>;

        nameOriginal = ko.observable("");
        nameRomaji = ko.observable("");
        nameEnglish = ko.observable("");
        pv1 = ko.observable("");
        pv2 = ko.observable("");
        songType = ko.observable("Original");

        hasName: KnockoutComputed<boolean>;

        public submit = () => {
            this.submitting(true);
            return true;
        };

        public submitting = ko.observable(false);

        removeArtist: (artist: dc.ArtistContract) => void;

        constructor(private songRepository: vdb.repositories.SongRepository, artistRepository: vdb.repositories.ArtistRepository, data?) {

            if (data) {
                this.nameOriginal(data.nameOriginal || "");
                this.nameRomaji(data.nameRomaji || "");
                this.nameEnglish(data.nameEnglish || "");
                this.pv1(data.pvUrl || "");
                this.pv2(data.reprintPVUrl || "");
                this.artists(data.artists || []);
            }

            this.addArtist = (artistId: number) => {

                if (artistId) {
                    artistRepository.getOne(artistId, artist => {
                        this.artists.push(artist);
						this.checkDuplicates();
                    });
                }

            };

            this.artistSearchParams = {
                allowCreateNew: false,
                acceptSelection: this.addArtist,
                extraQueryParams: { artistTypes: "Vocaloid,UTAU,OtherVocalist,OtherVoiceSynthesizer,Producer,Circle,OtherGroup,Unknown,Animator,Illustrator,Lyricist,OtherIndividual,Utaite,Band" },
                height: 300
            };

            this.hasName = ko.computed(() => {
                return this.nameOriginal().length > 0 || this.nameRomaji().length > 0 || this.nameEnglish().length > 0;
            });

            this.isDuplicatePV = ko.computed(() => {
                return _.some(this.dupeEntries(), item => { return item.matchProperty == 'PV' });
            });
            
            this.removeArtist = (artist: dc.ArtistContract) => {
                this.artists.remove(artist);
            };
            
            if (this.pv1()) {
                this.checkDuplicatesAndPV();
            }

        }
    
    }

}