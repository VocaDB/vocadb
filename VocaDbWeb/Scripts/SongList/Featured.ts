import RepositoryFactory from '@Repositories/RepositoryFactory';
import HttpClient from '@Shared/HttpClient';
import vdb from '@Shared/VdbStatic';
import FeaturedSongListsViewModel from '@ViewModels/SongList/FeaturedSongListsViewModel';
import $ from 'jquery';
import ko from 'knockout';
import moment from 'moment';

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

    var lang = vdb.values.languagePreference;
    const httpClient = new HttpClient();
    var repoFactory = new RepositoryFactory(httpClient);
    var songListRepo = repoFactory.songListRepository();
    var resourceRepo = repoFactory.resourceRepository();
    var tagRepo = repoFactory.tagRepository();
    var viewModel = new FeaturedSongListsViewModel(
      songListRepo,
      resourceRepo,
      tagRepo,
      lang,
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
