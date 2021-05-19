import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import ResourceRepository from '@Repositories/ResourceRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import ActivityEntryListViewModel from '@ViewModels/ActivityEntry/ActivityEntryListViewModel';
import $ from 'jquery';
import moment from 'moment';

const ActivityEntryIndex = (): void => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var resourceRepo = new ResourceRepository(
      httpClient,
      vdb.values.baseAddress,
    );
    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    var cultureCode = vdb.values.uiCulture;

    var vm = new ActivityEntryListViewModel(
      urlMapper,
      resourceRepo,
      languageSelection,
      cultureCode,
    );
    ko.applyBindings(vm);
  });
};

export default ActivityEntryIndex;
