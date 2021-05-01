import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import ResourceRepository from '../Repositories/ResourceRepository';
import UrlMapper from '../Shared/UrlMapper';
import ActivityEntryListViewModel from '../ViewModels/ActivityEntry/ActivityEntryListViewModel';

const UserEntryEdits = (
  additionsOnly: boolean,
  model: {
    id: number;
  },
) => {
  $(function () {
    moment.locale(vdb.values.culture);
    ko.punches.enableAll();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var resourceRepo = new ResourceRepository(vdb.values.baseAddress);
    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    var cultureCode = vdb.values.uiCulture;
    var userId = model.id;

    var vm = new ActivityEntryListViewModel(
      urlMapper,
      resourceRepo,
      languageSelection,
      cultureCode,
      userId,
      additionsOnly,
    );
    ko.applyBindings(vm);
  });
};

export default UserEntryEdits;
