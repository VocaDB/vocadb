import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import TagRepository from '../Repositories/TagRepository';
import UserRepository from '../Repositories/UserRepository';
import UrlMapper from '../Shared/UrlMapper';
import TagEditViewModel from '../ViewModels/TagEditViewModel';

function initPage() {
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const TagEdit = (model: { id: number }) => {
  $(document).ready(function () {
    initPage();

    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(vdb.values.baseAddress);
    var userRepo = new UserRepository(urlMapper);

    tagRepo.getById(
      model.id,
      'AliasedTo,TranslatedDescription,Names,Parent,RelatedTags,WebLinks',
      ContentLanguagePreference[vdb.values.languagePreference],
      function (contract) {
        var viewModel = new TagEditViewModel(urlMapper, userRepo, contract);
        ko.applyBindings(viewModel);
      },
    );
  });
};

export default TagEdit;
