import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import ActivityEntryListViewModel from '@ViewModels/ActivityEntry/ActivityEntryListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const ActivityEntryIndex = (): void => {
	$(function () {
		moment.locale(vocaDbContext.culture);
		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vocaDbContext.baseAddress);
		var resourceRepo = repoFactory.resourceRepository();
		var lang = vocaDbContext.languagePreference;
		var cultureCode = vocaDbContext.uiCulture;

		var vm = new ActivityEntryListViewModel(
			urlMapper,
			resourceRepo,
			lang,
			cultureCode,
		);
		ko.applyBindings(vm);
	});
};

export default ActivityEntryIndex;
