import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ActivityEntryListViewModel from '@ViewModels/ActivityEntry/ActivityEntryListViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

const repoFactory = container.get(RepositoryFactory);

const ActivityEntryIndex = (): void => {
	$(function () {
		moment.locale(vdb.values.culture);
		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		var resourceRepo = repoFactory.resourceRepository();
		var lang = vdb.values.languagePreference;
		var cultureCode = vdb.values.uiCulture;

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
