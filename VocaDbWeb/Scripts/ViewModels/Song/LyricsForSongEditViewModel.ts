
namespace vdb.viewModels.songs {

	import cls = models;
	import dc = vdb.dataContracts;

	export class LyricsForSongEditViewModel {

		constructor(contract?: dc.songs.LyricsForSongContract) {
			
			if (contract) {
				this.id = ko.observable(contract.id);
				this.cultureCode = ko.observable(contract.cultureCode);
				this.language = ko.observable(contract.language);
				this.source = ko.observable(contract.source);
				this.translationType = ko.observable(contract.translationType);
				this.value = ko.observable(contract.value);
			} else {
				this.id = ko.observable(0);
				this.cultureCode = ko.observable("");
				this.language = ko.observable(vdb.models.globalization.ContentLanguageSelection[vdb.models.globalization.ContentLanguageSelection.Unspecified]);
				this.source = ko.observable("");
				this.translationType = ko.observable(cls.globalization.TranslationType[cls.globalization.TranslationType.Translation]);
				this.value = ko.observable("");
			}

			this.isNew = contract == null;

		}

		public toggleAccordion = (vm, event: JQueryEventObject) => {
			var elem = $(event.target).closest(".accordion-group").find(".accordion-body") as any;
			elem.collapse('toggle');
		}

		public cultureCode: KnockoutObservable<string>;

		public id: KnockoutObservable<number>;

		public isNew: boolean;

		public language: KnockoutObservable<string>;

		public showLanguageSelection = () => this.translationType() !== cls.globalization.TranslationType[cls.globalization.TranslationType.Romanized];

		public source: KnockoutObservable<string>;

		public translationType: KnockoutObservable<string>;

		public value: KnockoutObservable<string>;

	}

	export class LyricsForSongListEditViewModel extends BasicListEditViewModel<LyricsForSongEditViewModel, dc.songs.LyricsForSongContract> {

		private find = (translationType: string) => {
			var vm = _.find(this.items(), i => i.translationType() === translationType);
			if (vm)
				_.remove(this.items(), vm);
			else {
				vm = new LyricsForSongEditViewModel({ translationType: translationType });
			}
			return vm;
		}

		constructor(contracts: dc.songs.LyricsForSongContract[]) {
			super(LyricsForSongEditViewModel, contracts);
			this.original = this.find("Original");
			this.romanized = this.find("Romanized");
		}

		public changeToOriginal = (lyrics: LyricsForSongEditViewModel) => {
			this.original.id(lyrics.id());
			this.original.value(lyrics.value());
			this.original.cultureCode(lyrics.cultureCode());
			this.original.source(lyrics.source());
			this.items.remove(lyrics);
		}

		public changeToTranslation = (lyrics: LyricsForSongEditViewModel) => {

			if (lyrics === this.original) {

				var newLyrics = new LyricsForSongEditViewModel({
					id: this.original.id(), cultureCode: this.original.cultureCode(), source: this.original.source(),
					value: this.original.value(),
					translationType: cls.globalization.TranslationType[cls.globalization.TranslationType.Translation]
				});

				this.items.push(newLyrics);

				this.original.id(0);
				this.original.value("");
				this.original.cultureCode("");
				this.original.source("");

			} else {
				lyrics.translationType(cls.globalization.TranslationType[cls.globalization.TranslationType.Translation]);
			}

		}

		public original: LyricsForSongEditViewModel;
		public romanized: LyricsForSongEditViewModel;

		public toContracts: () => dc.songs.LyricsForSongContract[] = () => {
			var result = ko.toJS(_.chain([this.original, this.romanized]).concat(this.items()).filter(i => i.value()).value());
			return result;
		}

	}

}