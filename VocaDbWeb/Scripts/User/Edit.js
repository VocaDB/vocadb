
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

	initEntrySearch(artistAddName, artistAddList, "Artist", "../../api/artists",
		{
			allowCreateNew: false,
			acceptBtnElem: artistAddBtn,
			acceptSelection: acceptArtistSelection,
			autoHide: true,
			createOptionFirstRow: function (item) { return item.name + " (" + item.artistType + ")"; },
			createOptionSecondRow: function (item) { return item.additionalNames; },
			extraQueryParams: { nameMatchMode: 'Auto', lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference], fields: 'AdditionalNames' },
			termParamName: 'query',
			method: 'GET'
		});

	$(document).on("click", "a.artistRemove", function () {

		$(this).parent().parent().remove();
		return false;

	});

});