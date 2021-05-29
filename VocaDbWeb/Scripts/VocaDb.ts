import functions from '@Shared/GlobalFunctions';
import vdb from '@Shared/VdbStatic';
import $ from 'jquery';

vdb.functions = vdb.functions || {};
vdb.values = vdb.values || {};

vdb.functions.disableTabReload = function (tab): void {
  tab.find('a').attr('href', '#' + tab.attr('aria-controls'));
};

vdb.functions.showLoginPopup = function (): void {
  $.get(
    '/User/LoginForm',
    { returnUrl: window.location.href },
    function (result) {
      $('#loginPopup').html(result);
      $('#loginPopup').dialog('open');
    },
  );
};

(function ($): void {
  $.fn.vdbArtistToolTip = function (): void {
    this.each(function (this: any) {
      var elem = this;

      $(elem).qtip({
        content: {
          text: 'Loading...',
          ajax: {
            url: functions.mapAbsoluteUrl('/Artist/PopupContent'),
            type: 'GET',
            data: { id: $(elem).data('entryId') },
          },
        },
        position: {
          viewport: $(window),
        },
        style: {
          classes: 'tooltip-wide',
        },
      });
    });
  };
})($);

(function ($): void {
  $.fn.vdbAlbumToolTip = function (): void {
    this.each(function (this: any) {
      var elem = this;

      $(elem).qtip({
        content: {
          text: 'Loading...',
          ajax: {
            url: functions.mapAbsoluteUrl('/Album/PopupContent'),
            type: 'GET',
            data: { id: $(elem).data('entryId') },
          },
        },
        position: {
          viewport: $(window),
        },
      });
    });
  };
})($);

(function ($): void {
  $.fn.vdbAlbumWithCoverToolTip = function (): void {
    this.each(function (this: any) {
      var elem = this;

      $(elem).qtip({
        content: {
          text: 'Loading...',
          ajax: {
            url: functions.mapAbsoluteUrl('/Album/PopupWithCoverContent'),
            type: 'GET',
            data: { id: $(elem).data('entryId') },
          },
        },
        position: {
          viewport: $(window),
        },
      });
    });
  };
})($);

$(document).ready(function () {
  $('#loginPopup').dialog({ autoOpen: false, width: 400, modal: true });
});
