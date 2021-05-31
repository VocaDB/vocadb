import UserRepository from '@Repositories/UserRepository';
import HttpClient from '@Shared/HttpClient';
import ui from '@Shared/MessagesTyped';
import vdb from '@Shared/VdbStatic';
import NewsListViewModel from '@ViewModels/NewsListViewModel';
import PVRatingButtonsViewModel from '@ViewModels/PVRatingButtonsViewModel';
import $ from 'jquery';
import ko from 'knockout';

declare global {
  interface JQuery {
    scrollable: (params: any) => void;
  }
}

function initPage(): void {
  function initRatingButtons(): void {
    const httpClient = new HttpClient();
    const repo = new UserRepository(httpClient);
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

  $('#songs-navi .scrollable-item').click(function (this: any, e) {
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

const HomeIndex = (blogUrl: string): void => {
  $(function () {
    var viewModel = new NewsListViewModel(blogUrl);
    ko.applyBindings(viewModel);

    initPage();
  });
};

export default HomeIndex;
