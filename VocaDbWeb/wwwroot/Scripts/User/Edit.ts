import ArtistContract from '../DataContracts/Artist/ArtistContract';
import ContentLanguagePreference from '../Models/Globalization/ContentLanguagePreference';
import functions from '../Shared/GlobalFunctions';
import { initEntrySearch } from '../Shared/EntryAutoComplete';

function initPage(): void {
  function artistAdded(row): void {
    var artistsTable = $('#ownedArtistsTableBody');
    artistsTable.append(row);
  }

  function acceptArtistSelection(artistId, term): void {
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
    createOptionSecondRow: (item: ArtistContract) => item.additionalNames,
    extraQueryParams: {
      nameMatchMode: 'Auto',
      lang: ContentLanguagePreference[vdb.values.languagePreference],
      fields: 'AdditionalNames',
    },
    termParamName: 'query',
  });

  $(document).on('click', 'a.artistRemove', function () {
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
