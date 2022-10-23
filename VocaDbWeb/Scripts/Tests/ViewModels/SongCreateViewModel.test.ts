import { ArtistApiContract } from '@/DataContracts/Artist/ArtistApiContract';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { SongType } from '@/Models/Songs/SongType';
import { FakeArtistRepository } from '@/Tests/TestSupport/FakeArtistRepository';
import { FakeSongRepository } from '@/Tests/TestSupport/FakeSongRepository';
import { FakeTagRepository } from '@/Tests/TestSupport/FakeTagRepository';
import { SongCreateViewModel } from '@/ViewModels/SongCreateViewModel';

var repository = new FakeSongRepository();
var artistRepository = new FakeArtistRepository();
var tagRepository = new FakeTagRepository();
var producer: ArtistApiContract = {
	artistType: ArtistType.Producer,
	id: 1,
	name: 'Tripshots',
	additionalNames: '',
	status: undefined!,
	mainPicture: undefined!,
	version: undefined!,
};
artistRepository.result = producer;
repository.results = {
	title: 'Nebula',
	titleLanguage: 'English',
	artists: [producer],
	matches: [],
	songType: SongType.Original,
};

function createViewModel(): SongCreateViewModel {
	return new SongCreateViewModel(
		vdb.values,
		repository,
		artistRepository,
		tagRepository,
	);
}

test('constructor empty', () => {
	var target = createViewModel();

	expect(target.nameOriginal(), 'nameOriginal').toBe('');
	expect(target.nameEnglish(), 'nameEnglish').toBe('');
	expect(target.pv1(), 'pv1').toBe('');
	expect(target.artists(), 'artists').toBeTruthy();
	expect(target.artists().length, 'artists.length').toBe(0);
});

test('constructor with data', () => {
	var target = new SongCreateViewModel(
		vdb.values,
		repository,
		artistRepository,
		tagRepository,
		{ nameEnglish: 'Nebula', artists: [producer] },
	);

	expect(target.nameEnglish(), 'nameEnglish').toBe('Nebula');
	expect(target.artists(), 'artists').toBeTruthy();
	expect(target.artists().length, 'artists.length').toBe(1);
	expect(target.artists()[0].id, 'artist id').toBe(1);
});

test('addArtist', () => {
	var target = createViewModel();

	target.addArtist(1);

	expect(target.artists().length, 'artists.length').toBe(1);
	expect(target.artists()[0].id, 'artist id').toBe(1);
});

test('checkDuplicatesAndPV title and artists', () => {
	var target = createViewModel();

	target.checkDuplicatesAndPV();

	expect(target.nameEnglish(), 'nameEnglish').toBe('Nebula');
	expect(target.artists(), 'artists').toBeTruthy();
	expect(target.artists().length, 'artists.length').toBe(1);
});

test('checkDuplicatesAndPV does not overwrite title', () => {
	var target = createViewModel();
	target.nameOriginal('Overridden title');

	target.checkDuplicatesAndPV();

	expect(target.nameOriginal(), 'nameOriginal').toBe('Overridden title');
});
