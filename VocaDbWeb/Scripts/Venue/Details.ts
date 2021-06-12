import RepositoryFactory from '@Repositories/RepositoryFactory';
import VocaDbContext from '@Shared/VocaDbContext';
import { container } from '@Shared/inversify.config';
import { IEntryReportType } from '@ViewModels/ReportEntryViewModel';
import VenueDetailsViewModel from '@ViewModels/Venue/VenueDetailsViewModel';
import $ from 'jquery';
import ko from 'knockout';

const vocaDbContext = container.get(VocaDbContext);
const repoFactory = container.get(RepositoryFactory);

const VenueDetails = (
	model: {
		id: number;
	},
	reportTypes: IEntryReportType[],
): void => {
	$(function () {
		$('#editVenueLink').button({
			disabled: $('#editVenueLink').hasClass('disabled'),
			icons: { primary: 'ui-icon-wrench' },
		});
		$('#viewVersions').button({ icons: { primary: 'ui-icon-clock' } });
		$('#createEventLink').button({ icons: { primary: 'ui-icon-plus' } });
		$('#reportEntryLink').button({ icons: { primary: 'ui-icon-alert' } });

		var loggedUserId = vocaDbContext.loggedUserId;
		var venueRepo = repoFactory.venueRepository();

		var vm = new VenueDetailsViewModel(
			venueRepo,
			reportTypes,
			loggedUserId,
			model.id,
		);
		ko.applyBindings(vm);
	});
};

export default VenueDetails;
