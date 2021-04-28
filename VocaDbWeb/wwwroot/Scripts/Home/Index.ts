import PVRatingButtonsViewModel from '../ViewModels/PVRatingButtonsViewModel';
import ui from '../Shared/MessagesTyped';
import UrlMapper from '../Shared/UrlMapper';
import UserRepository from '../Repositories/UserRepository';
import NewsListViewModel from '../ViewModels/NewsListViewModel';
import HttpClient from '../Shared/HttpClient';

declare global {
  interface JQuery {
    scrollable: (params: any) => void;
  }
}

function initPage() {
  function initRatingButtons() {
    const httpClient = new HttpClient();
    const urlMapper = new UrlMapper(vdb.values.baseAddress);
    const repo = new UserRepository(httpClient, urlMapper);
    const ratingBar = $('#rating-bar');

    if (!ratingBar.length) {
      return;
    }

    const songId = ratingBar.data('song-id');
    const rating = ratingBar.data('rating');
    const viewModel = new PVRatingButtonsViewModel(
      repo,
      { id: songId, vote: rating },
      () => {
        ui.showSuccessMessage(vdb.resources.song.thanksForRating);
      },
      vdb.values.isLoggedIn,
    );
    ko.applyBindings(viewModel, ratingBar[0]);
  }

  initRatingButtons();

  $('#songs-navi .scrollable-item').click(function (e) {
    e.preventDefault();

    $('#songs-navi .scrollable-item').removeClass('active');
    $(this).addClass('active');

    const songId = $(this).find('.js-songId').val();
    $.post('../Home/PVContent', { songId: songId }, (content: string) => {
      $('#songPreview').html(content);
      initRatingButtons();
    });
  });

  $('#songs-navi .scrollable-item').eq(0).addClass('active');
  $('.scrollable').scrollable({
    vertical: true,
    mousewheel: true,
    keyboard: false,
  });

  $('#newAlbums img').vdbAlbumToolTip();
  $('#topAlbums img').vdbAlbumToolTip();
}

const HomeIndex = (blogUrl: string) => {
  $(function () {
    var viewModel = new NewsListViewModel(blogUrl);
    ko.applyBindings(viewModel);

    initPage();
  });
};

export default HomeIndex;
