import VenueForEditContract from '@DataContracts/Venue/VenueForEditContract';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import { container } from '@Shared/inversify.config';
import VenueEditViewModel from '@ViewModels/Venue/VenueEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
	$('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
	$('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const VenueEdit = (model: VenueForEditContract): void => {
	$(function () {
		var urlMapper = new UrlMapper(vdb.values.baseAddress);
		const repoFactory = container.get(RepositoryFactory);
		var venueRepo = repoFactory.venueRepository();
		var userRepo = repoFactory.userRepository();
		var contract = model;

		var vm = new VenueEditViewModel(venueRepo, userRepo, urlMapper, contract);
		ko.applyBindings(vm);

		initPage();
	});
};

export default VenueEdit;
