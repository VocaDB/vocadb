
namespace vdb.viewModels.songs {

	import dc = vdb.dataContracts;

	export class LyricsForSongEditViewModel {

		constructor(contract?: dc.songs.LyricsForSongContract) {
			
			if (contract) {
				this.id = contract.id;
				this.cultureCode = ko.observable(contract.cultureCode);
				this.language = ko.observable(contract.language);
				this.source = ko.observable(contract.source);
				this.translationType = ko.observable(contract.translationType);
				this.value = ko.observable(contract.value);
			} else {
				this.cultureCode = ko.observable("");
				this.language = ko.observable(vdb.models.globalization.ContentLanguageSelection[vdb.models.globalization.ContentLanguageSelection.Unspecified]);
				this.source = ko.observable("");
				this.translationType = ko.observable("Translation");
				this.value = ko.observable("");
			}

			this.translationType("Translation");
			this.isNew = contract == null;

		}

		public toggleAccordion = (vm, event: JQueryEventObject) => {
			var elem = $(event.target).closest(".accordion-group").find(".accordion-body") as any;
			elem.collapse('toggle');
		}

		public cultureCode: KnockoutObservable<string>;

		public id: number;

		public isNew: boolean;

		public language: KnockoutObservable<string>;

		public source: KnockoutObservable<string>;

		public translationType: KnockoutObservable<string>;

		public value: KnockoutObservable<string>;

	}

	export class LyricsForSongListEditViewModel extends BasicListEditViewModel<LyricsForSongEditViewModel, dc.songs.LyricsForSongContract> {

		constructor(contracts: dc.songs.LyricsForSongContract[]) {
			super(LyricsForSongEditViewModel, contracts);
			this.original = new LyricsForSongEditViewModel();
			this.original.translationType('Original');
			this.romanized = new LyricsForSongEditViewModel();
			this.romanized.translationType('Romanized');
		}

		public original: LyricsForSongEditViewModel;
		public romanized: LyricsForSongEditViewModel;

	}

}