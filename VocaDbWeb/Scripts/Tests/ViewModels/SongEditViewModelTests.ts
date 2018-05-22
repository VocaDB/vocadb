/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/Song/SongEditViewModel.ts" />

module vdb.tests.viewModels {

	import cls = vdb.models;
	import vm = vdb.viewModels;
	import dc = vdb.dataContracts;
	import sup = vdb.tests.testSupport;

	var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
	var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
	var data: dc.songs.SongForEditContract;
	var songRepo = new sup.FakeSongRepository();
	var artistRepo = new sup.FakeArtistRepository();
	var pvRepo = null;
	var userRepo = new sup.FakeUserRepository();
	resources.song = { addExtraArtist: 'Add extra artist' };

	function addArtist(viewModel: vm.SongEditViewModel, artistType: cls.artists.ArtistType, roles: cls.artists.ArtistRoles) {
		const artist = { id: 39, name: 'Clean Tears', artistType: cls.artists.ArtistType[artistType] };
		viewModel.artistLinks.push(new vm.ArtistForAlbumEditViewModel(null, { artist: artist, isSupport: false, roles: vdb.helpers.ArtistHelper.getRolesList(roles) }));
	}

	QUnit.module("SongEditViewModelTests", {
		setup: () => {
			data = {
				artists: [], defaultNameLanguage: 'English', deleted: false, id: 0, lengthSeconds: 39,
				hasAlbums: false,
				lyrics: [], names: [],
				notes: { original: '', english: '' },
				originalVersion: null, pvs: [], songType: 'Original',
				status: 'Draft', tags: [], webLinks: [webLinkData]
			};			
		}
    });

    function createViewModel() {
		return new vm.SongEditViewModel(songRepo, artistRepo, pvRepo, userRepo, new vdb.UrlMapper(''), {}, categories, data, false, null, 0, null);
    }

    QUnit.test("constructor", () => {

        var target = createViewModel();

        equal(target.length(), 39, "length");
        equal(target.lengthFormatted(), "0:39", "lengthFormatted");
        equal(target.webLinks.webLinks().length, 1, "webLinks.length");
		equal(target.validationError_duplicateArtist(), false, "validationError_duplicateArtist");

    });

	QUnit.test("lengthFormatted only seconds", () => {

        var target = createViewModel();

        target.lengthFormatted("39");
        
        equal(target.length(), 39, "length");

    });

	QUnit.test("lengthFormatted over 1 minute", () => {

        var target = createViewModel();

        target.lengthFormatted("393");

        equal(target.length(), 393, "length");

    });

	QUnit.test("lengthFormatted minutes and seconds", () => {

        var target = createViewModel();

        target.lengthFormatted("3:39");

        equal(target.length(), 219, "length");

    });

	QUnit.test("firstPvDate no PV", () => {

        var target = createViewModel();

		QUnit.equal(target.firstPvDate(), null, "firstPvDate");

	});

	QUnit.test("firstPvDate with PVs", () => {

		data.pvs = [
			{ pvType: 'Original', pvId: '3', service: 'YouTube' },
			{ publishDate: '2039-3-8', pvType: 'Reprint', pvId: '39', service: 'YouTube' },
			{ publishDate: '2039-3-9', pvType: 'Original', pvId: '3939', service: 'YouTube' },
			{ publishDate: '2039-3-10', pvType: 'Original', pvId: '3939', service: 'YouTube' }
		];

        var target = createViewModel();

		QUnit.ok(target.firstPvDate(), "firstPvDate");
		QUnit.equal(target.firstPvDate().toISOString(), moment('2039-3-9').toISOString(), "firstPvDate");

	});

	QUnit.test("suggestedPublishDate no date", () => {

		var target = createViewModel();
		QUnit.equal(target.suggestedPublishDate(), null, "suggestedPublishDate");

	});

	QUnit.test("suggestedPublishDate with album date", () => {

		data.albumReleaseDate = '2039-3-9';

		var target = createViewModel();

		QUnit.ok(target.suggestedPublishDate(), "suggestedPublishDate");
		QUnit.equal(target.suggestedPublishDate().date.toISOString(), moment('2039-3-9').toISOString(), "suggestedPublishDate");

	});

	QUnit.test("validationError_duplicateArtist", () => {

        var target = createViewModel();
		var artist = new vm.ArtistForAlbumEditViewModel(null, { artist: { id: 1, name: '164' }, roles: '' });

		target.artistLinks.push(artist);
		target.artistLinks.push(artist);

		equal(target.validationError_duplicateArtist(), true, "validationError_duplicateArtist");

    });

	QUnit.test("validationError_duplicateArtist support", () => {

		const target = createViewModel();

		const artist = { id: 39, name: 'Clean Tears' };
		target.artistLinks.push(new vm.ArtistForAlbumEditViewModel(null, { artist: artist, isSupport: false, roles: '' }));
		target.artistLinks.push(new vm.ArtistForAlbumEditViewModel(null, { artist: artist, isSupport: true, roles: '' }));

		equal(target.validationError_duplicateArtist(), true, "validationError_duplicateArtist");

	});

	QUnit.test("validationError_redundantRoles false", () => {

		const target = createViewModel();
		addArtist(target, cls.artists.ArtistType.Producer, cls.artists.ArtistRoles.Default);

		equal(target.validationError_redundantRoles(), false, "validationError_redundantRoles");

	});

	QUnit.test("validationError_redundantRoles true", () => {

		const target = createViewModel();
		addArtist(target, cls.artists.ArtistType.Producer, cls.artists.ArtistRoles.Composer);

		equal(target.validationError_redundantRoles(), true, "validationError_redundantRoles");

	});

}