
function initDialog(urlMapper) {

	function addTag(tagName) {

		if (isNullOrWhiteSpace(tagName))
			return;

		$("#newTagName").val("");

		if ($("#tagSelection_" + tagName).length) {
			$("#tagSelection_" + tagName).prop('checked', true);
			$("#tagSelection_" + tagName).button("refresh");
			return;
		}

		$.post(urlMapper.mapRelative("/Tag/Create"), { name: tagName }, function (response) {

			if (!response.Successful) {
				alert(response.Message);
			} else {
				$("#tagSelections").append(response.Result);
				$("input.tagSelection").button();
			}

		});

	}

	$("input.tagSelection").button();

	$("input#newTagName").autocomplete({
		source: urlMapper.mapRelative("/Tag/Find"),
		select: function (event, ui) { addTag(ui.item.label); return false; }
	});

	$("#addNewTag").click(function () {

		addTag($("#newTagName").val());
		return false;

	});

}

function tabLoaded(albumId, event, ui, confirmDeleteStr) {

	vdb.functions.disableTabReload(ui.tab);

	$("#createComment").click(function () {

		var message = $("#newCommentMessage").val();

		if (message == "") {
			alert("Message cannot be empty");
			return false;
		}

		$("#newCommentMessage").val("");

		$.post(urlMapper.mapRelative("/Album/CreateComment"), { entryId: albumId, message: message }, function (result) {

			$("#newCommentPanel").after(result);

		});

		return false;

	});

	$(document).on("click", "a.deleteComment", function () {

		if (!confirm(confirmDeleteStr))
			return false;

		var btn = this;
		var id = getId(this);

		$.post(urlMapper.mapRelative("/Album/DeleteComment"), { commentId: id }, function () {

			$(btn).parent().parent().parent().parent().remove();

		});

		return false;

	});

}

function initPage(albumId, collectionRating, saveStr, confirmDeleteStr, urlMapper) {

	$("#addAlbumLink").button({ disabled: $("#addAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-star'} });
	$("#updateAlbumLink").button({ disabled: $("#updateAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#editAlbumLink").button({ disabled: $("#editAlbumLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#downloadTags").button({ icons: { primary: 'ui-icon-arrowthickstop-1-s' } })
		.next().button({ text: false, icons: { primary: "ui-icon-triangle-1-s" } }).parent().buttonset();
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
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

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections }] });

	$("#tabs").tabs({
		load: function(event, ui) {
			tabLoaded(albumId, event, ui, confirmDeleteStr);
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

	var addAlbumLinkPos;
	if ($("#addAlbumLink").is(":visible"))
		addAlbumLinkPos = $("#addAlbumLink").offset();
	else
		addAlbumLinkPos = $("#updateAlbumLink").offset();

	$("#editCollectionDialog").dialog("option", "position", [addAlbumLinkPos.left, addAlbumLinkPos.top + 35]);

	$("#addAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	$("#updateAlbumLink").click(function () {

		$("#editCollectionDialog").dialog("open");
		return false;

	});

	// TODO: not in use AFAIK
	$("#removeAlbumLink").click(function () {

		$.post(urlMapper.mapRelative("/User/RemoveAlbumFromUser"), { albumId: albumId }, function (result) {

			$("#addAlbumLink").show();
			$("#removeAlbumLink").hide();

		});

		return false;

	});

	initReportEntryPopup(saveStr, urlMapper.mapRelative("/Album/CreateReport"), { albumId: albumId });

	$("#editTags").click(function () {

		$.get(urlMapper.mapRelative("/Album/TagSelections"), { albumId: albumId }, function (content) {

			$("#editTagsAlbumId").val(albumId);
			$("#editTagsContent").html(content);

			initDialog(urlMapper);

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

	$("td.artistList a").vdbArtistToolTip();
	
	$("#userCollectionsPopup").dialog({ autoOpen: false, width: 400, position: { my: "left top", at: "left bottom", of: $("#statsLink") } });

	function saveTagSelections() {

		var tagNames = new Array();

		$("input.tagSelection:checked").each(function () {
			var name = $(this).parent().find("input.tagName").val();
			tagNames.push(name);
		});

		var tagNamesStr = tagNames.join(",");
		var tagsAlbumId = $("#editTagsAlbumId").val();

		$.post(urlMapper.mapRelative("/Album/TagSelections"), { albumId: tagsAlbumId, tagNames: tagNamesStr }, function (content) {

			$("#tagList").html(content);

		});

		$("#editTagsPopup").dialog("close");

	}

}