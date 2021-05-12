import ArtistContract from '@DataContracts/Artist/ArtistContract';
import TagRepository from '@Repositories/TagRepository';
import SongCreateViewModel from '@ViewModels/SongCreateViewModel';

import FakeArtistRepository from '../TestSupport/FakeArtistRepository';
import FakeSongRepository from '../TestSupport/FakeSongRepository';

var repository = new FakeSongRepository();
var artistRepository = new FakeArtistRepository();
var tagRepository: TagRepository = null!;
var producer: ArtistContract = {
  artistType: 'Producer',
  id: 1,
  name: 'Tripshots',
  additionalNames: '',
};
artistRepository.result = producer;
repository.results = {
  title: 'Nebula',
  titleLanguage: 'English',
  artists: [producer],
  matches: [],
  songType: 'Original',
};

QUnit.module('SongCreateViewModelTests');

function createViewModel(): SongCreateViewModel {
  return new SongCreateViewModel(repository, artistRepository, tagRepository);
}

test('constructor empty', () => {
  var target = createViewModel();

  equal(target.nameOriginal(), '', 'nameOriginal');
  equal(target.nameEnglish(), '', 'nameEnglish');
  equal(target.pv1(), '', 'pv1');
  ok(target.artists(), 'artists');
  equal(target.artists().length, 0, 'artists.length');
});

test('constructor with data', () => {
  var target = new SongCreateViewModel(
    repository,
    artistRepository,
    tagRepository,
    { nameEnglish: 'Nebula', artists: [producer] },
  );

  equal(target.nameEnglish(), 'Nebula', 'nameEnglish');
  ok(target.artists(), 'artists');
  equal(target.artists().length, 1, 'artists.length');
  equal(target.artists()[0].id, 1, 'artist id');
});

test('addArtist', () => {
  var target = createViewModel();

  target.addArtist(1);

  equal(target.artists().length, 1, 'artists.length');
  equal(target.artists()[0].id, 1, 'artist id');
});

test('checkDuplicatesAndPV title and artists', () => {
  var target = createViewModel();

  target.checkDuplicatesAndPV();

  equal(target.nameEnglish(), 'Nebula', 'nameEnglish');
  ok(target.artists(), 'artists');
  equal(target.artists().length, 1, 'artists.length');
});

test('checkDuplicatesAndPV does not overwrite title', () => {
  var target = createViewModel();
  target.nameOriginal('Overridden title');

  target.checkDuplicatesAndPV();

  equal(target.nameOriginal(), 'Overridden title', 'nameOriginal');
});
