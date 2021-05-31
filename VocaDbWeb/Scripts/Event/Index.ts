import $ from 'jquery';

const EventIndex = (): void => {
	$(function () {
		$('#createSeriesLink').button({ icons: { primary: 'ui-icon-plus' } });
		$('#createEventLink').button({ icons: { primary: 'ui-icon-plus' } });
		$('#createVenueLink').button({ icons: { primary: 'ui-icon-plus' } });
	});
};

export default EventIndex;
