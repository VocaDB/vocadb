/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../DataContracts/TranslatedEnumField.ts" />
/// <reference path="../../ViewModels/WebLinksEditViewModel.ts" />

module vdb.tests.viewModels {

    import dc = vdb.dataContracts;
    import vm = vdb.viewModels;

    var categories: dc.TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots" };

    QUnit.module("WebLinksEditViewModel");

    test("constructor", () => {

        var target = new vm.WebLinksEditViewModel([webLinkData], categories);

        equal(target.webLinks().length, 1, "webLinks.length");
        equal(target.categories.length, 2, "categories.length");

    });

    test("add new", () => {

        var target = new vm.WebLinksEditViewModel([]);

        target.add();

        equal(target.webLinks().length, 1, "webLinks.length");

    });

    test("remove", () => {

        var target = new vm.WebLinksEditViewModel([webLinkData]);

        target.remove(target.webLinks()[0]);

        equal(target.webLinks().length, 0, "webLinks.length");

    });

}