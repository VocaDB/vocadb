import { SongForEditContract } from '@/DataContracts/Song/SongForEditContract';
import { TranslatedEnumField } from '@/DataContracts/TranslatedEnumField';
import { ArtistType } from '@/Models/Artists/ArtistType';
import { EntryStatus } from '@/Models/EntryStatus';
import { PVService } from '@/Models/PVs/PVService';
import { PVType } from '@/Models/PVs/PVType';
import { SongType } from '@/Models/Songs/SongType';
import { WebLinkCategory } from '@/Models/WebLinkCategory';
import { UrlMapper } from '@/Shared/UrlMapper';
import { FakeArtistRepository } from '@/Tests/TestSupport/FakeArtistRepository';
import { FakeSongRepository } from '@/Tests/TestSupport/FakeSongRepository';
import { FakeUserRepository } from '@/Tests/TestSupport/FakeUserRepository';
import { ArtistForAlbumEditViewModel } from '@/ViewModels/ArtistForAlbumEditViewModel';
import { SongEditViewModel } from '@/ViewModels/Song/SongEditViewModel';
import moment from 'moment';

var categories: TranslatedEnumField[] = [
	{ id: 'Official', name: 'Official' },
	{ id: 'Commercial', name: 'Commercial' },
];
var webLinkData = {
	category: WebLinkCategory.Official,
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
vdb.resources = {
	album: {},
	albumDetails: { download: '' },
	albumEdit: { addExtraArtist: '' },
	entryEdit: {},
	shared: null,
	song: { addExtraArtist: 'Add extra artist' },
	artist: {},
	home: {},
	layout: {},
};

beforeEach(() => {
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
		songType: SongType.Original,
		status: EntryStatus.Draft,
		tags: [],
		webLinks: [webLinkData],
	};
});

function createViewModel(): SongEditViewModel {
	return new SongEditViewModel(
		vdb.values,
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

test('constructor', () => {
	var target = createViewModel();

	expect(target.length(), 'length').toBe(39);
	expect(target.lengthFormatted(), 'lengthFormatted').toBe('0:39');
	expect(target.webLinks.items().length, 'webLinks.length').toBe(1);
	expect(
		target.validationError_duplicateArtist(),
		'validationError_duplicateArtist',
	).toBe(false);
});

test('lengthFormatted only seconds', () => {
	var target = createViewModel();

	target.lengthFormatted('39');

	expect(target.length(), 'length').toBe(39);
});

test('lengthFormatted over 1 minute', () => {
	var target = createViewModel();

	target.lengthFormatted('393');

	expect(target.length(), 'length').toBe(393);
});

test('lengthFormatted minutes and seconds', () => {
	var target = createViewModel();

	target.lengthFormatted('3:39');

	expect(target.length(), 'length').toBe(219);
});

test('firstPvDate no PV', () => {
	var target = createViewModel();

	expect(target.firstPvDate(), 'firstPvDate').toBeUndefined();
});

test('firstPvDate with PVs', () => {
	data.pvs = [
		{
			id: 0,
			pvType: PVType.Original,
			pvId: '3',
			service: PVService.Youtube,
		},
		{
			id: 0,
			publishDate: '2039-03-08',
			pvType: PVType.Reprint,
			pvId: '39',
			service: PVService.Youtube,
		},
		{
			id: 0,
			publishDate: '2039-03-09',
			pvType: PVType.Original,
			pvId: '3939',
			service: PVService.Youtube,
		},
		{
			id: 0,
			publishDate: '2039-03-10',
			pvType: PVType.Original,
			pvId: '3939',
			service: PVService.Youtube,
		},
	];

	var target = createViewModel();

	expect(target.firstPvDate(), 'firstPvDate').toBeTruthy();
	expect(target.firstPvDate().toISOString(), 'firstPvDate').toBe(
		moment('2039-03-09').toISOString(),
	);
});

test('suggestedPublishDate no date', () => {
	var target = createViewModel();
	expect(target.suggestedPublishDate(), 'suggestedPublishDate').toBeUndefined();
});

test('suggestedPublishDate with album date', () => {
	data.albumReleaseDate = '2039-03-09';

	var target = createViewModel();

	expect(target.suggestedPublishDate(), 'suggestedPublishDate').toBeTruthy();
	expect(
		target.suggestedPublishDate().date.toISOString(),
		'suggestedPublishDate',
	).toBe(moment('2039-03-09').toISOString());
});

test('validationError_duplicateArtist', () => {
	var target = createViewModel();
	var artist = new ArtistForAlbumEditViewModel(null!, {
		artist: { id: 1, name: '164', artistType: ArtistType.Unknown },
		roles: '',
	});

	target.artistLinks.push(artist);
	target.artistLinks.push(artist);

	expect(
		target.validationError_duplicateArtist(),
		'validationError_duplicateArtist',
	).toBe(true);
});

test('validationError_duplicateArtist support', () => {
	const target = createViewModel();

	const artist = {
		id: 39,
		name: 'Clean Tears',
		artistType: ArtistType.Unknown,
	};
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

	expect(
		target.validationError_duplicateArtist(),
		'validationError_duplicateArtist',
	).toBe(true);
});
