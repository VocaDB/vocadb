import TagUsageForApiContract from '@DataContracts/Tag/TagUsageForApiContract';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
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

    var loggedUserId = vdb.values.loggedUserId;
    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var userRepo = new UserRepository(httpClient, urlMapper, loggedUserId);
    var tags = model.tags;

    var vm = new EventSeriesDetailsViewModel(userRepo, model.id, tags);
    ko.applyBindings(vm);
  });
};

export default EventSeriesDetails;
