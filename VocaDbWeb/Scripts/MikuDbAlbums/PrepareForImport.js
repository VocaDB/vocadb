
function initPage() {

    function acceptArtistSelection(albumId) {

        $.post("../../Album/Name", { id: albumId }, function (name) {

            $("#mergedAlbumId").append("<option value='" + albumId + "'>" + name + "</option>");
            $("#mergedAlbumId").val(albumId);
            $("#updateAlbumBtn").click();

        });

    }

    var artistAddList = $("#albumSearchList");
    var artistAddName = $("input#albumSearchName");
    var artistAddBtn = $("#albumSearchAcceptBtn");

    initEntrySearch(artistAddName, artistAddList, "Album", "../../Album/FindJson",
		{
		    acceptBtnElem: artistAddBtn,
		    acceptSelection: acceptArtistSelection,
		    createOptionFirstRow: function (item) { return item.Name; },
		    createOptionSecondRow: function (item) { return item.AdditionalNames; }
		});

}