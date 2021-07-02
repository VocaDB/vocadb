import ArtistContract from '@DataContracts/Artist/ArtistContract';
import { initEntrySearch } from '@Shared/EntryAutoComplete';
import functions from '@Shared/GlobalFunctions';
import $ from 'jquery';

function initPage(): void {
	function artistAdded(row: any): void {
		var artistsTable = $('#ownedArtistsTableBody');
		artistsTable.append(row);
	}

	function acceptArtistSelection(artistId: any, term: any): void {
		if (!functions.isNullOrWhiteSpace(artistId)) {
			$.post(
				'../../User/OwnedArtistForUserEditRow',
				{ artistId: artistId },
				artistAdded,
			);
		}
	}

	$('#clearRatingsLink').button();

	var artistAddName = $('input#ownedArtistAddName')[0];

	initEntrySearch(artistAddName, '../../api/artists', {
		acceptSelection: acceptArtistSelection,
		createOptionFirstRow: (item: ArtistContract) =>
			item.name + ' (' + item.artistType + ')',
		createOptionSecondRow: (item: ArtistContract) => item.additionalNames!,
		extraQueryParams: {
			nameMatchMode: 'Auto',
			lang: vdb.values.languagePreference,
			fields: 'AdditionalNames',
		},
		termParamName: 'query',
	});

	$(document).on('click', 'a.artistRemove', function (this: any) {
		$(this).parent().parent().remove();
		return false;
	});
}

const UserEdit = (): void => {
	$(document).ready(function () {
		initPage();
	});
};

export default UserEdit;
