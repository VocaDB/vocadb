
function getId(elem) {

	if ($(elem) == null || $(elem).attr('id') == null)
		return null;

	var parts = $(elem).attr('id').split("_");
	return (parts.length >= 2 ? parts[1] : null);
}

function isNullOrWhiteSpace(str) {

	if (str == null || str.length == 0)
		return true;

	return !(/\S/.test(str));

}

vdb = {};
vdb.functions = vdb.functions || {};
vdb.values = vdb.values || {};

vdb.functions.disableTabReload = function (tab) {
	tab.find("a").attr("href", "#" + tab.attr("aria-controls"));
};

vdb.functions.showLoginPopup = function() {

	$.get("/User/LoginForm", { returnUrl: window.location.href }, function(result) {
		$("#loginPopup").html(result);
		$("#loginPopup").dialog("open");
	});

};


(function ($) {
    $.fn.vdbArtistToolTip = function () {

        this.each(function () {
            var elem = this;

            $(elem).qtip({
                content: {
                    text: 'Loading...',
                    ajax: {
                    	url: app.functions.mapFullUrl('/Artist/PopupContent'),
                        type: 'GET',
                        data: { id: $(elem).data("entryId") }
                    }
                },
                position: {
                    viewport: $(window)
                },
                style: {
                    classes: "tooltip-wide"
                }
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
						url: app.functions.mapFullUrl('/Album/PopupContent'),
                        type: 'GET',
                        data: { id: $(elem).data("entryId") }
                    }
                },
                position: {
                    viewport: $(window)
                }
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
						url: app.functions.mapFullUrl('/Album/PopupWithCoverContent'),
						type: 'GET',
						data: { id: $(elem).data("entryId") }
					}
				},
				position: {
					viewport: $(window)
				}
			});
		});

	};
})(jQuery);

$(document).ready(function() {

	$("#loginPopup").dialog({ autoOpen: false, width: 400, modal: true });

});