
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
		source: function (ui, callback) {
			$.getJSON(urlMapper.mapRelative("/api/tags/names"), { query: ui.term }, callback);
		},
		select: function (event, ui) { addTag(ui.item.label); return false; }
	});

	$("#addNewTag").click(function () {

		addTag($("#newTagName").val());
		return false;

	});

}

function initPage(albumId, collectionRating, saveStr, urlMapper, viewModel) {

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