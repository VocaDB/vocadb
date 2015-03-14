
function initPage(artistId, saveStr, urlMapper, viewModel) {

	$("#addToUserLink").button({ disabled: $("#addToUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-heart'} });
	$("#removeFromUserLink").button({ disabled: $("#removeFromUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-close' } })
		.next().button({ text: false, icons: { primary: "ui-icon-triangle-1-s" } }).parent().buttonset();
	$("#editArtistLink").button({ disabled: $("#editArtistLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench' } });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("option", "active", 1);
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

	if (window.location.hash == "#mainAlbumsTab") {
		viewModel.initMainAlbums();
	}
	if (window.location.hash == "#collaborationAlbumsTab") {
		viewModel.initCollaborationAlbums();
	}
	if (window.location.hash == "#songsTab") {
		viewModel.initSongs();
	}

	$("#addToUserLink").click(function () {

		$.post(urlMapper.mapRelative("/User/AddArtistForUser"), { artistId: artistId }, function (result) {

			$("#removeFromUserSplitBtn").show();
			$("#addToUserLink").hide();

		});

		return false;

	});

	$("#removeFromUserLink").click(function () {

		$.post(urlMapper.mapRelative("/User/RemoveArtistFromUser"), { artistId: artistId }, function (result) {

			$("#addToUserLink").show();
			$("#removeFromUserSplitBtn").hide();

		});

		return false;

	});

	$("#newAlbums img").vdbAlbumToolTip();
	$("#topAlbums img").vdbAlbumToolTip();
	$("#baseVoicebank a").vdbArtistToolTip();
	$("#childVoicebanks a").vdbArtistToolTip();
	$("#groups a").vdbArtistToolTip();
	$(".artistLink").vdbArtistToolTip();

}