function addOption(select, value, text) {

	$(select).append("<option value=\"" + value + "\">" + text + "</option>");

}

function formatSongName(songContract) {

	return (songContract.Name + (songContract.ArtistString != "" ? " (by " + songContract.ArtistString + ")" : ""));

}

function getId(elem) {

	if ($(elem) == null || $(elem).attr('id') == null)
		return null;

	var parts = $(elem).attr('id').split("_");
	return (parts.length >= 2 ? parts[1] : null);
}

function htmlDecode(value) {
	if (value) {
		return $('<div />').html(value).text();
	} else {
		return '';
	}
}

function isNullOrWhiteSpace(str) {

	if (str == null || str.length == 0)
		return true;

	return !(/\S/.test(str));

}

String.prototype.trim = function () {
    return this.replace(/^\s*/, "").replace(/\s*$/, "");
};

vdb = {};
vdb.functions = vdb.functions || {};
vdb.values = vdb.values || {};

vdb.functions.disableTabReload = function (tab) {
	tab.find("a").attr("href", "#" + tab.attr("aria-controls"));
};

vdb.functions.mapUrl = function (relative) {
    return vdb.values.hostAddress + relative;
};

vdb.functions.boldCaseInsensitive = function (text, term) {

	if (!text || !term)
		return text;

	var index = text.toLowerCase().indexOf(term.toLowerCase());

	if (index < 0)
		return text;

	var actualTerm = text.substring(index, index + term.length);

	return text.replace(actualTerm, "<b>" + actualTerm + "</b>");

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
                        url: vdb.functions.mapUrl('/Artist/PopupContent'),
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
                        url: vdb.functions.mapUrl('/Album/PopupContent'),
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
						url: vdb.functions.mapUrl('/Album/PopupWithCoverContent'),
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