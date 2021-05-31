import TagRepository from '@Repositories/TagRepository';
import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import vdb from '@Shared/VdbStatic';
import TagEditViewModel from '@ViewModels/TagEditViewModel';
import $ from 'jquery';
import ko from 'knockout';

function initPage(): void {
  $('#trashLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#deleteLink').button({ icons: { primary: 'ui-icon-trash' } });
  $('#restoreLink').button({ icons: { primary: 'ui-icon-trash' } });
}

const TagEdit = (model: { id: number }): void => {
  $(document).ready(function () {
    initPage();

    const httpClient = new HttpClient();
    var urlMapper = new UrlMapper(vdb.values.baseAddress);
    var tagRepo = new TagRepository(httpClient);
    var userRepo = new UserRepository(httpClient);

    tagRepo
      .getById(
        model.id,
        'AliasedTo,TranslatedDescription,Names,Parent,RelatedTags,WebLinks',
        vdb.values.languagePreference,
      )
      .then(function (contract) {
        var viewModel = new TagEditViewModel(urlMapper, userRepo, contract);
        ko.applyBindings(viewModel);
      });
  });
};

export default TagEdit;
