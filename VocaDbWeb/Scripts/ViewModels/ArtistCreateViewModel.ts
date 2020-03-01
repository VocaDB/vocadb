import ArtistRepository from '../Repositories/ArtistRepository';
import ArtistType from '../Models/Artists/ArtistType';
import DuplicateEntryResultContract from '../DataContracts/DuplicateEntryResultContract';
import EntryType from '../Models/EntryType';
import EntryUrlMapper from '../Shared/EntryUrlMapper';
import TagApiContract from '../DataContracts/Tag/TagApiContract';
import TagRepository from '../Repositories/TagRepository';
import WebLinkEditViewModel from './WebLinkEditViewModel';

//module vdb.viewModels {

    export default class ArtistCreateViewModel {

		artistType = ko.observable(ArtistType[ArtistType.Producer]);
		artistTypeTag = ko.observable<TagApiContract>(null);
		artistTypeName = ko.computed(() => this.artistTypeTag()?.name);
		artistTypeInfo = ko.computed(() => this.artistTypeTag()?.description);
		artistTypeTagUrl = ko.computed(() => EntryUrlMapper.details_tag_contract(this.artistTypeTag()));

        public checkDuplicates: () => void;
        
        public dupeEntries = ko.observableArray<DuplicateEntryResultContract>([]);

		private getArtistTypeTag = async (artistType: string) => {
			const tag = await this.tagRepository.getEntryTypeTag(EntryType.Artist, artistType);
			this.artistTypeTag(tag);
		}

        public nameOriginal = ko.observable("");
        public nameRomaji = ko.observable("");
        public nameEnglish = ko.observable("");

        public submit = () => {
            this.submitting(true);
            return true;
        }

        public submitting = ko.observable(false);

        public webLink: WebLinkEditViewModel = new WebLinkEditViewModel();

		constructor(
			artistRepository: ArtistRepository,
			private readonly tagRepository: TagRepository,
			data?) {
            
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

			this.artistType.subscribe(this.getArtistTypeTag);
			this.getArtistTypeTag(this.artistType());

        }
    
    }

//}