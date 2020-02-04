
import functions from "../Shared/GlobalFunctions";

export function initPage(jsonModel, songId, saveStr, urlMapper, viewModel) {

	function initMediaPlayer() {
		$('audio').mediaelementplayer({
			pluginPath: 'https://cdnjs.com/libraries/mediaelement/'
		});
	}

    $(".js-ratingButtons").buttonset();
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock' } });
	$("#viewCommentsLink").click(function () {
		var index = $('#tabs ul [data-tab="Discussion"]').index();
		$("#tabs").tabs("option", "active", index);
		return false;
	});
	$("#viewRelatedLink").click(function () {
		var index = $('#tabs ul [data-tab="Related"]').index();
		$("#tabs").tabs("option", "active", index);
		return false;
	});

	$("#tabs").tabs({
		load: function (event, ui) {
			vdb.functions.disableTabReload(ui.tab);
		},
		activate: function (event, ui) {
			if (ui.newTab.data('tab') === "Discussion") {
				viewModel.comments.initComments();
			}
		}
	});

	$("#pvLoader")
		.ajaxStart(function () { $(this).show(); })
		.ajaxStop(function () { $(this).hide(); });

	$(".pvLink:not(.disabled)").click(function () {

		var id = functions.getId(this);
		$.post(urlMapper.mapRelative("/Song/PVForSong"), { pvId: id }, function (content) {
			$("#pvPlayer").html(content);
			initMediaPlayer();
		});

		return false;

	});

	$("td.artistList a").vdbArtistToolTip();
	$("#albumList a").vdbAlbumWithCoverToolTip();
	initMediaPlayer();

}