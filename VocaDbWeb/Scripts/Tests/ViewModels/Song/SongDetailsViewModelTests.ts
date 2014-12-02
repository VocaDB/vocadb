/// <reference path="../../../typings/qunit/qunit.d.ts" />
/// <reference path="../../TestSupport/FakeSongRepository.ts" />
/// <reference path="../../TestSupport/FakeUserRepository.ts" />
/// <reference path="../../../ViewModels/Song/SongDetailsViewModel.ts" />

module vdb.tests.viewModels {

    import cls = vdb.models;
    import sup = vdb.tests.testSupport;
    import vm = vdb.viewModels;
    import dc = vdb.dataContracts;

    var rep: sup.FakeSongRepository;
    var userRep = new sup.FakeUserRepository();
    var res: vm.SongDetailsResources = { createNewList: "Create new list" };
    var data: vm.SongDetailsAjax = { id: 39, selectedLyricsId: 0, selectedPvId: 0, userRating: "Nothing" };

    var target: vm.SongDetailsViewModel;

    QUnit.module("SongDetailsViewModelTests", {
        setup: () => {
			rep = new sup.FakeSongRepository();
            rep.songLists = [{ id: 1, name: "Favorite Mikus" }];
            target = new vm.SongDetailsViewModel(rep, userRep, res, data, null);
        }
    });

    test("constructor", () => {

        equal(target.id, 39, "id");
        ok(target.songListDialog, "songListDialog");
        ok(target.userRating, "userRating");
        equal(target.userRating.rating(), cls.SongVoteRating['Nothing'], "userRating.rating");

    });

    test("showSongLists has lists", () => {

        target.songListDialog.showSongLists();

        equal(target.songListDialog.songLists().length, 2, "songListDialog.songLists.length");
        equal(target.songListDialog.selectedListId(), 1, "songListDialog.selectedListId");

    });

    test("showSongLists no lists", () => {

        rep.songLists = [];
        target.songListDialog.showSongLists();

        equal(target.songListDialog.songLists().length, 1, "songListDialog.songLists.length");
        equal(target.songListDialog.selectedListId(), 0, "songListDialog.selectedListId");

    });

    test("addSongToList", () => {

        target.songListDialog.addSongToList();

        equal(rep.addedSongId, 39, "rep.addedSongId");

    });

    test("songInListsDialog show", () => {

        target.songInListsDialog.show();

        equal(target.songInListsDialog.dialogVisible(), true, "songInListsDialog.dialogVisible");
        ok(target.songInListsDialog.contentHtml(), "songInListsDialog.contentHtml");

    });

}