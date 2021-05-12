import ContentLanguagePreference from '@Models/Globalization/ContentLanguagePreference';
import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import UrlMapper from '@Shared/UrlMapper';
import FeaturedSongListsViewModel from '@ViewModels/SongList/FeaturedSongListsViewModel';

const SongListFeatured = (
  categories: string[],
  model: {
    tagId: number[];
  },
): void => {
  moment.locale(vdb.values.culture);
  ko.punches.enableAll();

  $(function () {
    $('#createLink').button({ icons: { primary: 'ui-icon-plusthick' } });
    $('#importLink').button({ icons: { primary: 'ui-icon-plusthick' } });

    var cultureCode = vdb.values.uiCulture;
    var tagIds = model.tagId;

    var languageSelection =
      ContentLanguagePreference[vdb.values.languagePreference];
    const httpClient = new HttpClient();
    var rootPath = vdb.values.baseAddress;
    var urlMapper = new UrlMapper(rootPath);
    var repoFactory = new RepositoryFactory(
      httpClient,
      urlMapper,
      vdb.values.languagePreference,
      vdb.values.loggedUserId,
    );
    var songListRepo = repoFactory.songListRepository();
    var resourceRepo = repoFactory.resourceRepository();
    var tagRepo = repoFactory.tagRepository();
    var viewModel = new FeaturedSongListsViewModel(
      songListRepo,
      resourceRepo,
      tagRepo,
      languageSelection,
      cultureCode,
      tagIds,
      categories,
    );
    ko.applyBindings(viewModel);

    if (window.location.hash && window.location.hash.length >= 1) {
      viewModel.setCategory(window.location.hash.substr(1));
    }
  });
};

export default SongListFeatured;
