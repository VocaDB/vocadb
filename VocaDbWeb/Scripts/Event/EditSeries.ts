import LocalizedStringWithIdContract from '@DataContracts/Globalization/LocalizedStringWithIdContract';
import WebLinkContract from '@DataContracts/WebLinkContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ReleaseEventSeriesEditViewModel from '@ViewModels/ReleaseEvent/ReleaseEventSeriesEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const EventEditSeries = (model: {
	defaultNameLanguage: string;
	id: number;
	names: LocalizedStringWithIdContract[];
	webLinks: WebLinkContract[];
}): void => {
	$(function () {
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		const repoFactory = container.get(RepositoryFactory);
		var eventRepo = repoFactory.eventRepository();
		var userRepo = repoFactory.userRepository();

		var vm = new ReleaseEventSeriesEditViewModel(
			eventRepo,
			userRepo,
			urlMapper,
			model.id,
			model.defaultNameLanguage,
			model.names,
			model.webLinks,
		);
		ko.applyBindings(vm);

		initPage();
	});
};

export default EventEditSeries;
