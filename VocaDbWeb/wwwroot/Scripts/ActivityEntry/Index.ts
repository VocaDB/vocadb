import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import ResourceRepository from '../Repositories/ResourceRepository';
import UrlMapper from '../Shared/UrlMapper';
import ActivityEntryListViewModel from '../ViewModels/ActivityEntry/ActivityEntryListViewModel';

const ActivityEntryIndex = () => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var resourceRepo = new ResourceRepository(vdb.values.baseAddress);
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
