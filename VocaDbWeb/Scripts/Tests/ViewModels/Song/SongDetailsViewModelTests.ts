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
    var data: vm.SongDetailsAjax = { id: 39, selectedLyricsId: 0, selectedPvId: 0, songType: 'Original', tagUsages: [], userRating: "Nothing", latestComments: [] };

    var target: vm.SongDetailsViewModel;

    QUnit.module("SongDetailsViewModelTests", {
        setup: () => {
			rep = new sup.FakeSongRepository();
            rep.songLists = [
				{ id: 1, name: "Favorite Mikus", featuredCategory: "Nothing" },
				{ id: 2, name: "Favorite Lukas", featuredCategory: "Nothing" },
				{ id: 3, name: "Mikupa 2013", featuredCategory: "Concerts" }
			];
            target = new vm.SongDetailsViewModel(rep, userRep, res, false, data, [], 0, vdb.models.globalization.ContentLanguagePreference.Default, false, null);
        }
    });

    QUnit.test("constructor", () => {

        equal(target.id, 39, "id");
        ok(target.songListDialog, "songListDialog");
        ok(target.userRating, "userRating");
        equal(target.userRating.rating(), cls.SongVoteRating['Nothing'], "userRating.rating");

    });

    QUnit.test("showSongLists has lists", () => {

        target.songListDialog.showSongLists();

        equal(target.songListDialog.songLists().length, 2, "songListDialog.songLists.length");
        equal(target.songListDialog.songLists()[0].name, "Favorite Mikus", "songListDialog.songLists[0].name");
        equal(target.songListDialog.selectedListId(), 1, "songListDialog.selectedListId");

    });

    QUnit.test("showSongLists no lists", () => {

        rep.songLists = [];
        target.songListDialog.showSongLists();

        equal(target.songListDialog.songLists().length, 0, "songListDialog.songLists.length");
        equal(target.songListDialog.selectedListId(), null, "songListDialog.selectedListId");

    });

    QUnit.test("addSongToList", () => {

        target.songListDialog.showSongLists();

        target.songListDialog.addSongToList();

        equal(rep.addedSongId, 39, "rep.addedSongId");

    });

	QUnit.test("tabName featured lists tab", () => {

        target.songListDialog.showSongLists();

		target.songListDialog.tabName("Featured");

        equal(target.songListDialog.songLists().length, 1, "songListDialog.songLists.length");
        equal(target.songListDialog.songLists()[0].name, "Mikupa 2013", "songListDialog.songLists[0].name");

    });

	QUnit.test("songInListsDialog show", () => {

        target.songInListsDialog.show();

        equal(target.songInListsDialog.dialogVisible(), true, "songInListsDialog.dialogVisible");
        ok(target.songInListsDialog.contentHtml(), "songInListsDialog.contentHtml");

    });

}