import ReleaseEventContract from '@DataContracts/ReleaseEvents/ReleaseEventContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import ReleaseEventEditViewModel from '@ViewModels/ReleaseEvent/ReleaseEventEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
	$('#tabs').tabs();
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const EventEdit = (
	artistRoleJson: { [key: string]: string },
	model: ReleaseEventContract,
): void => {
	$(function () {
		ko.punches.enableAll();

		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		const repoFactory = container.get(RepositoryFactory);
		var eventRepo = repoFactory.eventRepository();
		var userRepo = repoFactory.userRepository();
		var pvRepo = repoFactory.pvRepository();
		var artistRepo = repoFactory.artistRepository();
		var contract = model;

		var vm = new ReleaseEventEditViewModel(
			eventRepo,
			userRepo,
			pvRepo,
			artistRepo,
			urlMapper,
			artistRoleJson,
			contract,
		);
		ko.applyBindings(vm);

		initPage();
	});
};

export default EventEdit;
