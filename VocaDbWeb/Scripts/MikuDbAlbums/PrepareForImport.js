
function initPage() {

    function acceptArtistSelection(albumId) {

        $.get("../../api/albums/" + albumId, { lang: vdb.values.languagePreference }, function (album) {

            $("#mergedAlbumId").append("<option value='" + albumId + "'>" + album.name + "</option>");
            $("#mergedAlbumId").val(albumId);
            $("#updateAlbumBtn").click();

        });

    }

    var artistAddName = $("input#albumSearchName");
    var artistAddBtn = $("#albumSearchAcceptBtn");

    vdb.initEntrySearch(artistAddName, "../../api/albums",
		{
		    acceptBtnElem: artistAddBtn,
		    acceptSelection: acceptArtistSelection,
		    createOptionFirstRow: function (item) { return item.name; },
		    createOptionSecondRow: function (item) { return item.additionalNames; },
		    extraQueryParams: { nameMatchMode: 'Auto', lang: vdb.models.globalization.ContentLanguagePreference[vdb.values.languagePreference] },
			termParamName: 'query',
			method: 'GET'
		});

}