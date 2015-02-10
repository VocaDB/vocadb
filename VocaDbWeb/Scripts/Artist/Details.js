
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

function initPage(artistId, saveStr, urlMapper, viewModel) {

	$("#addToUserLink").button({ disabled: $("#addToUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-heart'} });
	$("#removeFromUserLink").button({ disabled: $("#removeFromUserLink").hasClass("disabled"), icons: { primary: 'ui-icon-close' } })
		.next().button({ text: false, icons: { primary: "ui-icon-triangle-1-s" } }).parent().buttonset();
	$("#editArtistLink").button({ disabled: $("#editArtistLink").hasClass("disabled"), icons: { primary: 'ui-icon-wrench' } });
	$("#viewVersions").button({ icons: { primary: 'ui-icon-clock'} });
	$("#reportEntryLink").button({ icons: { primary: 'ui-icon-alert'} });
	$("#editTags").button({ disabled: $("#editTags").hasClass("disabled"), icons: { primary: 'ui-icon-tag'} });
	$("#manageTags").button({ icons: { primary: 'ui-icon-wrench' } });
	$("#viewCommentsLink").click(function () {
		$("#tabs").tabs("option", "active", 1);
		return false;
	});

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections}] });

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

	initReportEntryPopup(saveStr, urlMapper.mapRelative("/Artist/CreateReport"), { artistId: artistId });

	$("#editTags").click(function () {

		$.get(urlMapper.mapRelative("/Artist/TagSelections"), { artistId: artistId }, function (content) {

			$("#editTagsArtistId").val(artistId);
			$("#editTagsContent").html(content);

			initDialog(urlMapper);

			$("#editTagsPopup").dialog("open");

		});

		return false;

	});

	$("#newAlbums img").vdbAlbumToolTip();
	$("#topAlbums img").vdbAlbumToolTip();
	$("#baseVoicebank a").vdbArtistToolTip();
	$("#childVoicebanks a").vdbArtistToolTip();
	$("#groups a").vdbArtistToolTip();
	$(".artistLink").vdbArtistToolTip();

	function saveTagSelections() {

		var tagNames = new Array();

		$("input.tagSelection:checked").each(function () {
			var name = $(this).parent().find("input.tagName").val();
			tagNames.push(name);
		});

		var tagNamesStr = tagNames.join(",");
		var tagsArtistId = $("#editTagsArtistId").val();

		$.post(urlMapper.mapRelative("/Artist/TagSelections"), { artistId: tagsArtistId, tagNames: tagNamesStr }, function (content) {

			$("#tagList").html(content);

		});

		$("#editTagsPopup").dialog("close");

	}

}