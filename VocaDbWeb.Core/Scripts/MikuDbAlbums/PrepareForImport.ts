import AlbumContract from "../DataContracts/Album/AlbumContract";
import ContentLanguagePreference from "../Models/Globalization/ContentLanguagePreference";
import { initEntrySearch } from "../Shared/EntryAutoComplete";

export function initPage() {

    function acceptArtistSelection(albumId) {

        $.get("../../api/albums/" + albumId, { lang: vdb.values.languagePreference }, function (album) {

            $("#mergedAlbumId").append("<option value='" + albumId + "'>" + album.name + "</option>");
            $("#mergedAlbumId").val(albumId);
            $("#updateAlbumBtn").click();

        });

    }

    var artistAddName = $("input#albumSearchName")[0];

	initEntrySearch(artistAddName, "../../api/albums",
		{
			acceptSelection: acceptArtistSelection,
			createOptionFirstRow: (item: AlbumContract) => (item.name),
			createOptionSecondRow: (item: AlbumContract) => (item.additionalNames),
			extraQueryParams: { nameMatchMode: 'Auto', lang: ContentLanguagePreference[vdb.values.languagePreference] },
			termParamName: 'query'
		});

}