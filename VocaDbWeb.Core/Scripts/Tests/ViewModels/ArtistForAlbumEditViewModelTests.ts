import ArtistContract from '../../DataContracts/Artist/ArtistContract';
import ArtistForAlbumContract from '../../DataContracts/ArtistForAlbumContract';
import ArtistForAlbumEditViewModel from '../../ViewModels/ArtistForAlbumEditViewModel';
import FakeAlbumRepository from '../TestSupport/FakeAlbumRepository';

    var rep = new FakeAlbumRepository();
    var producer: ArtistContract;
    var data: ArtistForAlbumContract;

    QUnit.module("ArtistForAlbumEditViewModelTests", {
        setup: () => {
            producer = { id: 1, name: "Tripshots", additionalNames: "", artistType: "Producer" };
            data = { artist: producer, id: 39, isSupport: false, name: "", roles: "Default" };
        }
    });

    function createViewModel() {
        return new ArtistForAlbumEditViewModel(rep, data);
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