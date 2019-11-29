/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../TestSupport/FakeAlbumRepository.ts" />
/// <reference path="../../ViewModels/ArtistForAlbumEditViewModel.ts" />

//module vdb.tests.viewModels {

    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var rep = new vdb.tests.testSupport.FakeAlbumRepository();
    var producer: dc.ArtistContract;
    var data: dc.ArtistForAlbumContract;

    QUnit.module("ArtistForAlbumEditViewModelTests", {
        setup: () => {
            producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
            data = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
        }
    });

    function createViewModel() {
        return new vm.ArtistForAlbumEditViewModel(rep, data);
    }

    test("constructor", () => {

        var target = createViewModel();

        equal(target.isCustomizable(), true, "isCustomizable");
        equal(target.roles(), "Default", "roles");
        equal(target.rolesArray().length, 1, "rolesArray.length");
        equal(target.rolesArray()[0], "Default", "rolesArray[0]");

    });

    test("isCustomizable", () => {

        producer.artistType = "Vocaloid";
        var target = createViewModel();

        equal(target.isCustomizable(), false, "isCustomizable");

    });

    test("rolesArray write", () => {

        var target = createViewModel();

        target.rolesArray(['Composer', 'Arranger']);

        equal(target.roles(), "Composer,Arranger", "roles");

    });

    test("roles write", () => {

        var target = createViewModel();

        target.roles('Composer, Arranger');

        equal(target.rolesArray().length, 2, "rolesArray.length");
        equal(target.rolesArray()[0], "Composer", "rolesArray[0]");
        equal(target.rolesArray()[1], "Arranger", "rolesArray[1]");

    });

//}