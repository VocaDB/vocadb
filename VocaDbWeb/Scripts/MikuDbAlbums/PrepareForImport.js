
function initPage() {

    function acceptArtistSelection(albumId) {

        $.post("../../Album/Name", { id: albumId }, function (name) {

            $("#mergedAlbumId").append("<option value='" + albumId + "'>" + name + "</option>");
            $("#mergedAlbumId").val(albumId);
            $("#updateAlbumBtn").click();

        });

    }

    var artistAddName = $("input#albumSearchName");
    var artistAddBtn = $("#albumSearchAcceptBtn");

    vdb.initEntrySearch(artistAddName, "Album", "../../api/albums",
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