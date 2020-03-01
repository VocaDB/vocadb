import DeleteEntryViewModel from '../DeleteEntryViewModel';
import EntryType from '../../Models/EntryType';
import EntryUrlMapper from '../../Shared/EntryUrlMapper';
import LocalizedStringWithIdContract from '../../DataContracts/Globalization/LocalizedStringWithIdContract';
import NamesEditViewModel from '../Globalization/NamesEditViewModel';
import NameMatchMode from '../../Models/NameMatchMode';
import ReleaseEventRepository from '../../Repositories/ReleaseEventRepository';
import UrlMapper from '../../Shared/UrlMapper';
import UserRepository from '../../Repositories/UserRepository';
import WebLinkContract from '../../DataContracts/WebLinkContract';
import WebLinksEditViewModel from '../WebLinksEditViewModel';

//module vdb.viewModels.releaseEvents {

	export default class ReleaseEventSeriesEditViewModel {

		constructor(
			private readonly eventRepository: ReleaseEventRepository,
			userRepository: UserRepository,
			private readonly urlMapper: UrlMapper,
			private readonly id: number,
			defaultNameLanguage: string,
			names: LocalizedStringWithIdContract[],
			webLinks: WebLinkContract[]) {

			this.defaultNameLanguage = ko.observable(defaultNameLanguage);
			this.names = NamesEditViewModel.fromContracts(names);
			this.webLinks = new WebLinksEditViewModel(webLinks);			

			if (!this.isNew()) {
				window.setInterval(() => userRepository.refreshEntryEdit(EntryType.ReleaseEventSeries, id), 10000);
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

			this.eventRepository.getSeriesList(value, NameMatchMode.Exact, 1, result => {				
				this.duplicateName(result.items.length ? value : null);
			});

		}
		
		public defaultNameLanguage: KnockoutObservable<string>;
		public description = ko.observable<string>();
		public duplicateName = ko.observable<string>();
		public names: NamesEditViewModel;
		public submitting = ko.observable(false);
        public webLinks: WebLinksEditViewModel;

		public deleteViewModel = new DeleteEntryViewModel(notes => {
			this.eventRepository.deleteSeries(this.id, notes, false, () => {
				window.location.href = this.urlMapper.mapRelative(EntryUrlMapper.details(EntryType.ReleaseEventSeries, this.id));
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