
$(document).ready(function () {

	function acceptArtistSelection(artistId, term) {

		if (!isNullOrWhiteSpace(artistId)) {
			$.post("../../User/OwnedArtistForUserEditRow", { artistId: artistId }, artistAdded);
		}

	}

	function artistAdded(row) {

		var artistsTable = $("#ownedArtistsTableBody");
		artistsTable.append(row);

	}

	$("#clearRatingsLink").button();

	var artistAddList = $("#ownedArtistAddList");
	var artistAddName = $("input#ownedArtistAddName");
	var artistAddBtn = $("#ownedArtistAddAcceptBtn");

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../Artist/FindJson",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			autoHide: true,
			createOptionFirstRow: function (item) { return item.Name + " (" + item.ArtistType + ")"; },
			createOptionSecondRow: function (item) { return item.AdditionalNames; }
		});

	$(document).on("click", "a.artistRemove", function () {

		$(this).parent().parent().remove();
		return false;

	});

});