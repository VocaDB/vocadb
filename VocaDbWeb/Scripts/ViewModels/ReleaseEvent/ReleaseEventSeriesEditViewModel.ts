
module vdb.viewModels.releaseEvents {

	import rep = repositories;

	export class ReleaseEventSeriesEditViewModel {

		constructor(
			private readonly eventRepository: rep.ReleaseEventRepository,
			userRepository: rep.UserRepository,
			private readonly urlMapper: vdb.UrlMapper,
			private readonly id: number,
			defaultNameLanguage: string,
			names: dataContracts.globalization.LocalizedStringWithIdContract[],
			webLinks: dc.WebLinkContract[]) {

			this.defaultNameLanguage = ko.observable(defaultNameLanguage);
			this.names = globalization.NamesEditViewModel.fromContracts(names);
			this.webLinks = new WebLinksEditViewModel(webLinks);			

			if (id) {
				window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.ReleaseEventSeries, id), 10000);
			}

		}
		
		public defaultNameLanguage: KnockoutObservable<string>;
		public description = ko.observable<string>();
		public names: globalization.NamesEditViewModel;
		public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			this.eventRepository.deleteSeries(this.id, notes, () => {
				window.location.href = this.urlMapper.mapRelative(utils.EntryUrlMapper.details(models.EntryType.ReleaseEventSeries, this.id));
			});
		});

		public submit = () => {
			this.submitting(true);
			return true;
		}

	}

}