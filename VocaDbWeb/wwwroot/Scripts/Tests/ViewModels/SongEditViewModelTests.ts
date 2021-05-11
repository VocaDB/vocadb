import SongForEditContract from '@DataContracts/Song/SongForEditContract';
import TranslatedEnumField from '@DataContracts/TranslatedEnumField';
import ArtistHelper from '@Helpers/ArtistHelper';
import ArtistRoles from '@Models/Artists/ArtistRoles';
import ArtistType from '@Models/Artists/ArtistType';
import UrlMapper from '@Shared/UrlMapper';
import ArtistForAlbumEditViewModel from '@ViewModels/ArtistForAlbumEditViewModel';
import SongEditViewModel from '@ViewModels/Song/SongEditViewModel';

import FakeArtistRepository from '../TestSupport/FakeArtistRepository';
import FakeSongRepository from '../TestSupport/FakeSongRepository';
import FakeUserRepository from '../TestSupport/FakeUserRepository';

var categories: TranslatedEnumField[] = [
  { id: 'Official', name: 'Official' },
  { id: 'Commercial', name: 'Commercial' },
];
var webLinkData = {
  category: 'Official',
  description: 'Youtube Channel',
  id: 0,
  url: 'http://www.youtube.com/user/tripshots',
  disabled: false,
};
var data: SongForEditContract;
var songRepo = new FakeSongRepository();
var artistRepo = new FakeArtistRepository();
var pvRepo: any = null;
var userRepo = new FakeUserRepository();
vdb.resources.song = { addExtraArtist: 'Add extra artist' };

function addArtist(
  viewModel: SongEditViewModel,
  artistType: ArtistType,
  roles: ArtistRoles,
): void {
  const artist =
    artistType != null
      ? { id: 39, name: 'Clean Tears', artistType: ArtistType[artistType] }
      : null;
  viewModel.artistLinks.push(
    new ArtistForAlbumEditViewModel(null!, {
      artist: artist!,
      isSupport: false,
      roles: ArtistHelper.getRolesList(roles),
    }),
  );
}

QUnit.module('SongEditViewModelTests', {
  setup: () => {
    data = {
      artists: [],
      defaultNameLanguage: 'English',
      deleted: false,
      id: 0,
      lengthSeconds: 39,
      hasAlbums: false,
      lyrics: [],
      names: [],
      notes: { original: '', english: '' },
      originalVersion: null!,
      pvs: [],
      songType: 'Original',
      status: 'Draft',
      tags: [],
      webLinks: [webLinkData],
    };
  },
});

function createViewModel(): SongEditViewModel {
  return new SongEditViewModel(
    songRepo,
    artistRepo,
    pvRepo,
    userRepo,
    new UrlMapper(''),
    {},
    categories,
    data,
    false,
    null!,
    0,
    null,
  );
}

QUnit.test('constructor', () => {
  var target = createViewModel();

  equal(target.length(), 39, 'length');
  equal(target.lengthFormatted(), '0:39', 'lengthFormatted');
  equal(target.webLinks.items().length, 1, 'webLinks.length');
  equal(
    target.validationError_duplicateArtist(),
    false,
    'validationError_duplicateArtist',
  );
});

QUnit.test('lengthFormatted only seconds', () => {
  var target = createViewModel();

  target.lengthFormatted('39');

  equal(target.length(), 39, 'length');
});

QUnit.test('lengthFormatted over 1 minute', () => {
  var target = createViewModel();

  target.lengthFormatted('393');

  equal(target.length(), 393, 'length');
});

QUnit.test('lengthFormatted minutes and seconds', () => {
  var target = createViewModel();

  target.lengthFormatted('3:39');

  equal(target.length(), 219, 'length');
});

QUnit.test('firstPvDate no PV', () => {
  var target = createViewModel();

  QUnit.equal(target.firstPvDate(), null, 'firstPvDate');
});

QUnit.test('firstPvDate with PVs', () => {
  data.pvs = [
    { pvType: 'Original', pvId: '3', service: 'YouTube' },
    {
      publishDate: '2039-3-8',
      pvType: 'Reprint',
      pvId: '39',
      service: 'YouTube',
    },
    {
      publishDate: '2039-3-9',
      pvType: 'Original',
      pvId: '3939',
      service: 'YouTube',
    },
    {
      publishDate: '2039-3-10',
      pvType: 'Original',
      pvId: '3939',
      service: 'YouTube',
    },
  ];

  var target = createViewModel();

  QUnit.ok(target.firstPvDate(), 'firstPvDate');
  QUnit.equal(
    target.firstPvDate().toISOString(),
    moment('2039-3-9').toISOString(),
    'firstPvDate',
  );
});

QUnit.test('suggestedPublishDate no date', () => {
  var target = createViewModel();
  QUnit.equal(target.suggestedPublishDate(), null, 'suggestedPublishDate');
});

QUnit.test('suggestedPublishDate with album date', () => {
  data.albumReleaseDate = '2039-3-9';

  var target = createViewModel();

  QUnit.ok(target.suggestedPublishDate(), 'suggestedPublishDate');
  QUnit.equal(
    target.suggestedPublishDate().date.toISOString(),
    moment('2039-3-9').toISOString(),
    'suggestedPublishDate',
  );
});

QUnit.test('validationError_duplicateArtist', () => {
  var target = createViewModel();
  var artist = new ArtistForAlbumEditViewModel(null!, {
    artist: { id: 1, name: '164' },
    roles: '',
  });

  target.artistLinks.push(artist);
  target.artistLinks.push(artist);

  equal(
    target.validationError_duplicateArtist(),
    true,
    'validationError_duplicateArtist',
  );
});

QUnit.test('validationError_duplicateArtist support', () => {
  const target = createViewModel();

  const artist = { id: 39, name: 'Clean Tears' };
  target.artistLinks.push(
    new ArtistForAlbumEditViewModel(null!, {
      artist: artist,
      isSupport: false,
      roles: '',
    }),
  );
  target.artistLinks.push(
    new ArtistForAlbumEditViewModel(null!, {
      artist: artist,
      isSupport: true,
      roles: '',
    }),
  );

  equal(
    target.validationError_duplicateArtist(),
    true,
    'validationError_duplicateArtist',
  );
});
