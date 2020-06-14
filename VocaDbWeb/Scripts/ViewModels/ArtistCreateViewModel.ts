/// <reference path="../typings/jquery/jquery.d.ts" />
/// <reference path="../DataContracts/DuplicateEntryResultContract.ts" />
/// <reference path="../Repositories/ArtistRepository.ts" />
/// <reference path="WebLinkEditViewModel.ts" />

module vdb.viewModels {

	import cls = models;
    import dc = vdb.dataContracts;

    export class ArtistCreateViewModel {

		artistType = ko.observable(cls.artists.ArtistType[cls.artists.ArtistType.Producer]);
		artistTypeTag = ko.observable<dc.TagApiContract>(null);
		artistTypeName = ko.computed(() => this.artistTypeTag()?.name);
		artistTypeInfo = ko.computed(() => this.artistTypeTag()?.description);
		artistTypeTagUrl = ko.computed(() => vdb.utils.EntryUrlMapper.details_tag_contract(this.artistTypeTag()));

        public checkDuplicates: () => void;
        
        public dupeEntries = ko.observableArray<dc.DuplicateEntryResultContract>([]);

		private getArtistTypeTag = async (artistType: string) => {
			const tag = await this.tagRepository.getEntryTypeTag(cls.EntryType.Artist, artistType);
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
			artistRepository: vdb.repositories.ArtistRepository,
			private readonly tagRepository: vdb.repositories.TagRepository,
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

}