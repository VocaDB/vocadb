
function initPage(albumId, collectionRating, saveStr, urlMapper, viewModel) {

	$("#addAlbumLink").button({ disabled: $("#addAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-star'} });
	$("#updateAlbumLink").button({ disabled: $("#updateAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#editAlbumLink").button({ disabled: $("#editAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#downloadTags").button({ icons: { primary: 'ui-icon-arrowthickstop-1-s' } })
		.next().button({ text: false, icons: { primary: "ui-icon-triangle-1-s" } }).parent().buttonset();
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("option", "active", 1);
		return false;
	});

	$('#picCarousel').carousel({ interval: false });

	$("#collectionRating").jqxRating();

	if (collectionRating != 0) {
		$('#collectionRating').jqxRating({ value: collectionRating });
	}

	$("#removeRating").click(function () {

		$("#collectionRating").jqxRating('setValue', 0);

		return false;

	});

	$("#tabs").tabs({
		load: function(event, ui) {
			vdb.functions.disableTabReload(ui.tab);
		},
		activate: function (event, ui) {
			if (ui.newTab.data('tab') === "Discussion") {
				viewModel.comments.initComments();
			}
		}
	});

	$("#editCollectionDialog").dialog({ autoOpen: false, width: 320, modal: false, buttons: [{ text: saveStr, click: function () {

		$("#editCollectionDialog").dialog("close");

		var status = $("#collectionStatusSelect").val();
		var mediaType = $("#collectionMediaSelect").val();
		var rating = $("#collectionRating").jqxRating('getValue');

		$.post(urlMapper.mapRelative("/User/UpdateAlbumForUser"), { albumId: albumId, collectionStatus: status, mediaType: mediaType, rating: rating }, null);

		if (status == "Nothing") {
			$("#updateAlbumLink").hide();
			$("#addAlbumLink").show();
		} else {
			$("#addAlbumLink").hide();
			$("#updateAlbumLink").show();
		}

		vdb.ui.showSuccessMessage(vdb.resources.album.addedToCollection);

	}}]});

	var addAlbumLink;
	if ($("#addAlbumLink").is(":visible"))
		addAlbumLink = $("#addAlbumLink");
	else
		addAlbumLink = $("#updateAlbumLink");

	$("#editCollectionDialog").dialog("option", "position", { my: "left top", at: "left bottom", of: addAlbumLink });

	$("#addAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	$("#updateAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	initReportEntryPopup(saveStr, urlMapper.mapRelative("/Album/CreateReport"), { albumId: albumId });

	$("td.artistList a").vdbArtistToolTip();
	
	$("#userCollectionsPopup").dialog({ autoOpen: false, width: 400, position: { my: "left top", at: "left bottom", of: $("#statsLink") } });

}