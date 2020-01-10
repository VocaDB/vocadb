/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />
/// <reference path="../Repositories/ArtistRepository.ts" />
/// <reference path="WebLinkEditViewModel.ts" />

import ArtistRepository from '../Repositories/ArtistRepository';
import DuplicateEntryResultContract from '../DataContracts/DuplicateEntryResultContract';
import WebLinkEditViewModel from './WebLinkEditViewModel';

//module vdb.viewModels {

    export default class ArtistCreateViewModel {
        
        public checkDuplicates: () => void;
        
        public dupeEntries = ko.observableArray<DuplicateEntryResultContract>([]);

        public nameOriginal = ko.observable("");
        public nameRomaji = ko.observable("");
        public nameEnglish = ko.observable("");

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);

        public webLink: WebLinkEditViewModel = new WebLinkEditViewModel();

        constructor(artistRepository: ArtistRepository, data?) {
            
            if (data) {
                this.nameOriginal(data.nameOriginal || "");
                this.nameRomaji(data.nameRomaji || "");
                this.nameEnglish(data.nameEnglish || "");
            }

            this.checkDuplicates = () => {

                var term1 = this.nameOriginal();
                var term2 = this.nameRomaji();
                var term3 = this.nameEnglish();
                var linkUrl = this.webLink.url();

                artistRepository.findDuplicate({ term1: term1, term2: term2, term3: term3, linkUrl: linkUrl }, result => {
                    this.dupeEntries(result);
                });

            }

        }
    
    }

//}