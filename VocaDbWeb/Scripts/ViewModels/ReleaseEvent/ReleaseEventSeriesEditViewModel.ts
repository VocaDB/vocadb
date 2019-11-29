
//module vdb.viewModels.releaseEvents {

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

			if (!this.isNew()) {
				window.setInterval(() => userRepository.refreshEntryEdit(models.EntryType.ReleaseEventSeries, id), 10000);
			} else {
				_.forEach([this.names.originalName, this.names.romajiName, this.names.englishName], name => {
					ko.computed(() => name.value()).extend({ rateLimit: 500 }).subscribe(this.checkName);
				});
			}

		}

		private checkName = (value: string) => {

			if (!value) {
				this.duplicateName(null);
				return;				
			}

			this.eventRepository.getSeriesList(value, vdb.models.NameMatchMode.Exact, 1, result => {				
				this.duplicateName(result.items.length ? value : null);
			});

		}
		
		public defaultNameLanguage: KnockoutObservable<string>;
		public description = ko.observable<string>();
		public duplicateName = ko.observable<string>();
		public names: globalization.NamesEditViewModel;
		public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			this.eventRepository.deleteSeries(this.id, notes, false, () => {
				window.location.href = this.urlMapper.mapRelative(utils.EntryUrlMapper.details(models.EntryType.ReleaseEventSeries, this.id));
			});
		});

		private redirectToRoot = () => {
			window.location.href = this.urlMapper.mapRelative("Event");
		}

		public trashViewModel = new DeleteEntryViewModel(notes => {
			this.eventRepository.deleteSeries(this.id, notes, true, this.redirectToRoot);
		});

		private isNew = () => !this.id;

		public submit = () => {
			this.submitting(true);
			return true;
		}

	}

//}