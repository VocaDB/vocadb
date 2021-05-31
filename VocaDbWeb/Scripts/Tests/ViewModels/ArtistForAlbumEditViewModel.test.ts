import ArtistContract from '@DataContracts/Artist/ArtistContract';
import ArtistForAlbumContract from '@DataContracts/ArtistForAlbumContract';
import ArtistForAlbumEditViewModel from '@ViewModels/ArtistForAlbumEditViewModel';

import FakeAlbumRepository from '../TestSupport/FakeAlbumRepository';

var rep = new FakeAlbumRepository();
var producer: ArtistContract;
var data: ArtistForAlbumContract;

beforeEach(() => {
	producer = {
		id: 1,
		name: 'Tripshots',
		additionalNames: '',
		artistType: 'Producer',
	};
	data = {
		artist: producer,
		id: 39,
		isSupport: false,
		name: '',
		roles: 'Default',
	};
});

function createViewModel(): ArtistForAlbumEditViewModel {
	return new ArtistForAlbumEditViewModel(rep, data);
}

test('constructor', () => {
	var target = createViewModel();

	expect(target.isCustomizable(), 'isCustomizable').toBe(true);
	expect(target.roles(), 'roles').toBe('Default');
	expect(target.rolesArray().length, 'rolesArray.length').toBe(1);
	expect(target.rolesArray()[0], 'rolesArray[0]').toBe('Default');
});

test('isCustomizable', () => {
	producer.artistType = 'Vocaloid';
	var target = createViewModel();

	expect(target.isCustomizable(), 'isCustomizable').toBe(false);
});

test('rolesArray write', () => {
	var target = createViewModel();

	target.rolesArray(['Composer', 'Arranger']);

	expect(target.roles(), 'roles').toBe('Composer,Arranger');
});

test('roles write', () => {
	var target = createViewModel();

	target.roles('Composer, Arranger');

	expect(target.rolesArray().length, 'rolesArray.length').toBe(2);
	expect(target.rolesArray()[0], 'rolesArray[0]').toBe('Composer');
	expect(target.rolesArray()[1], 'rolesArray[1]').toBe('Arranger');
});
