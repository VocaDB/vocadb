
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

	$("#editTagsPopup").dialog({ autoOpen: false, width: 500, modal: true, buttons: [{ text: saveStr, click: saveTagSelections}] });

	initReportEntryPopup(saveStr, urlMapper.mapRelative("/Song/CreateReport"), { songId: songId });

	/*$("#editTags").click(function () {

		$.get(urlMapper.mapRelative("/Song/TagSelections"), { songId: songId }, function (content) {

			$("#editTagsSongId").val(songId);
			$("#editTagsContent").html(content);

			initDialog();

			$("#editTagsPopup").dialog("open");

			var pvPlayer = $("#pvPlayer");
			if (pvPlayer.length) {
				var pvPlayerBottom = pvPlayer.offset().top + pvPlayer.outerHeight() - $(window).scrollTop();
				var centerTop = (($(window).height() - $("#editTagsPopup").outerHeight()) / 2);
				var left = (($(window).width() - $("#editTagsPopup").outerWidth()) / 2);
				var top = Math.max(centerTop, pvPlayerBottom);
				$("#editTagsPopup").dialog("option", "position", [left, top]);
			}

		});

		return false;

	});*/

	$(".pvLink").click(function () {

		var id = getId(this);
		$.post(urlMapper.mapRelative("/Song/PVForSong"), { pvId: id }, function (content) {
			$("#pvPlayer").html(content);
		});

		return false;

	});

	function initDialog() {

		$("input.tagSelection").button();

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

	/*function saveTagSelections() {

		var tagNames = new Array();

		$("input.tagSelection:checked").each(function () {
			var name = $(this).parent().find("input.tagName").val();
			tagNames.push(name);
		});

		var tagNamesStr = tagNames.join(",");

		$.post(urlMapper.mapRelative("/Song/TagSelections"), { songId: songId, tagNames: tagNamesStr }, function (content) {

			$("#tagList").html(content);

		});

		$("#editTagsPopup").dialog("close");

	}*/

	$("td.artistList a").vdbArtistToolTip();
	$("#albumList a").vdbAlbumWithCoverToolTip();

}