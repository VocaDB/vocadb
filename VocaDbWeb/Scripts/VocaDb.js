vdb = {};
vdb.values = vdb.values || {};

(function ($) {
	$.fn.vdbArtistToolTip = function () {
		this.each(function () {
			var elem = this;

			$(elem).qtip({
				content: {
					text: 'Loading...',
					ajax: {
						url: app.functions.mapAbsoluteUrl('/Artist/PopupContent'),
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
})(jQuery);

(function ($) {
	$.fn.vdbAlbumToolTip = function () {
		this.each(function () {
			var elem = this;

			$(elem).qtip({
				content: {
					text: 'Loading...',
					ajax: {
						url: app.functions.mapAbsoluteUrl('/Album/PopupContent'),
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
})(jQuery);

(function ($) {
	$.fn.vdbAlbumWithCoverToolTip = function () {
		this.each(function () {
			var elem = this;

			$(elem).qtip({
				content: {
					text: 'Loading...',
					ajax: {
						url: app.functions.mapAbsoluteUrl('/Album/PopupWithCoverContent'),
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
})(jQuery);

$(document).ready(function () {
	$('#loginPopup').dialog({ autoOpen: false, width: 400, modal: true });
});
