
module vdb.viewModels {

    import dc = vdb.dataContracts;

    export class RequestVerificationViewModel {
    
        constructor(private artistRepository: vdb.repositories.ArtistRepository) {}

        public clearArtist = () => {
            this.selectedArtist(null);
        }

        public selectedArtist: KnockoutObservable<dc.ArtistContract> = ko.observable(null);

        public setArtist = (targetArtistId) => {

            this.artistRepository.getOne(targetArtistId, artist => {
                this.selectedArtist(artist);
            });

        }

        public artistSearchParams: vdb.knockoutExtensions.AutoCompleteParams = {
            allowCreateNew: false,
            acceptSelection: this.setArtist,
            height: 300
        };
      
    }

}