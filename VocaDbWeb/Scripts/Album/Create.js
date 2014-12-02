
function initPage() {

	$("input.nameField").focusout(function () {

		var term1 = $("#nameOriginal").val();
		var term2 = $("#nameRomaji").val();
		var term3 = $("#nameEnglish").val();

		$.post("../../Album/FindDuplicate", { term1: term1, term2: term2, term3: term3 }, function (result) {

			if (result != "Ok") {
				$("#duplicateEntryWarning").html(result);
				$("#duplicateEntryWarning").show();
				$("#duplicateEntryWarning a").vdbAlbumWithCoverToolTip();
			} else {
				$("#duplicateEntryWarning").hide();
			}

		});

	});

	function acceptArtistSelection(artistId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../Artist/CreateArtistContractRow", { artistId: artistId }, function (row) {
				var artistsTable = $("#artistsTableBody");
				artistsTable.append(row);
				$("#artistsTableBody a.artistLink:last").vdbArtistToolTip();
			});
		}

	}

	var artistAddList = $("#artistAddList");
	var artistAddName = $("input#artistAddName");
	var artistAddBtn = $("#artistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; },
			height: 300
		});

	$(document).on("click", "a.artistRemove", function () {

		$(this).parent().parent().remove();
		return false;

	});

	$("#artistsTableBody a.artistLink").vdbArtistToolTip();

}