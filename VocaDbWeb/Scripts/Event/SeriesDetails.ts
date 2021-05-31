import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import EventSeriesDetailsViewModel from '@ViewModels/ReleaseEvent/EventSeriesDetailsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const EventSeriesDetails = (model: {
	id: number;
	tags: TagUsageForApiContract[];
}): void => {
	$(function () {
		$('#editEventLink').button({ icons: { primary: 'ui-icon-wrench' } });
		$('#createEventLink').button({ icons: { primary: 'ui-icon-plus' } });
		$('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });

		const httpClient = new HttpClient();
		var userRepo = new UserRepository(httpClient);
		var tags = model.tags;

		var vm = new EventSeriesDetailsViewModel(userRepo, model.id, tags);
		ko.applyBindings(vm);
	});
};

export default EventSeriesDetails;
