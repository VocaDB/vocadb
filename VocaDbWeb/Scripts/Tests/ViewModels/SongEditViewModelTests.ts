/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../ViewModels/Song/SongEditViewModel.ts" />

module vdb.tests.viewModels {

	import vm = vdb.viewModels;
	import dc = vdb.dataContracts;
	import sup = vdb.tests.testSupport;

	var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
	var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };
	var data: dc.songs.SongForEditContract = {
		artists: [], defaultNameLanguage: 'English', deleted: false, id: 0, lengthSeconds: 39,
		hasAlbums: false,
		lyrics: [], names: [],
		notes: { original: '', english: '' },
		originalVersion: null, pvs: [], songType: 'Original',
		status: 'Draft', tags: [], webLinks: [webLinkData]
	};
	var songRepo = new sup.FakeSongRepository();
	var artistRepo = new sup.FakeArtistRepository();
	var pvRepo = null;
	var userRepo = new sup.FakeUserRepository();
	resources.song = { addExtraArtist: 'Add extra artist' };

    QUnit.module("SongEditViewModelTests");

    function createViewModel() {
		return new vm.SongEditViewModel(songRepo, artistRepo, pvRepo, userRepo, new vdb.UrlMapper(''), {}, categories, data, false, null, 0);
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

	QUnit.test("validationError_duplicateArtist", () => {

        var target = createViewModel();
		var artist = new vm.ArtistForAlbumEditViewModel(null, { artist: { id: 1, name: '164' }, roles: '' });

		target.artistLinks.push(artist);
		target.artistLinks.push(artist);

		equal(target.validationError_duplicateArtist(), true, "validationError_duplicateArtist");

    });

}