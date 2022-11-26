import { AlbumContract } from '@/DataContracts/Album/AlbumContract';
import { initEntrySearch } from '@/Shared/EntryAutoComplete';
import $ from 'jquery';

function initPage(): void {
	function acceptArtistSelection(albumId?: number): void {
		$.get(
			'../../api/albums/' + albumId,
			{ lang: (window as any).vdb.values.languagePreference },
			function (album) {
				$('#mergedAlbumId').append(
					"<option value='" + albumId + "'>" + album.name + '</option>',
				);
				$('#mergedAlbumId').val(`${albumId}`);
				$('#updateAlbumBtn').click();
			},
		);
	}

	var artistAddName = $('input#albumSearchName')[0];

	initEntrySearch(artistAddName, '../../api/albums', {
		acceptSelection: acceptArtistSelection,
		createOptionFirstRow: (item: AlbumContract) => item.name,
		createOptionSecondRow: (item: AlbumContract) => item.additionalNames,
		extraQueryParams: {
			nameMatchMode: 'Auto',
			lang: (window as any).vdb.values.languagePreference,
		},
		termParamName: 'query',
	});
}

export const MikuDbAlbumPrepareForImport = (): void => {
	$(document).ready(function () {
		initPage();
	});
};
