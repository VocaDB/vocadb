
function initPage(jsonModel, songId, saveStr, urlMapper, viewModel) {

    $(".js-ratingButtons").buttonset();
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock' } });
	$("#viewCommentsLink").click(function () {
		var index = $('#tabs ul #discussionTabLink').index();
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

	initReportEntryPopup(saveStr, urlMapper.mapRelative("/Song/CreateReport"), { songId: songId });

	$(".pvLink").click(function () {

		var id = getId(this);
		$.post(urlMapper.mapRelative("/Song/PVForSong"), { pvId: id }, function (content) {
			$("#pvPlayer").html(content);
		});

		return false;

	});

	$("td.artistList a").vdbArtistToolTip();
	$("#albumList a").vdbAlbumWithCoverToolTip();

}