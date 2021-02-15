import TranslatedEnumField from '../../DataContracts/TranslatedEnumField';
import WebLinksEditViewModel from '../../ViewModels/WebLinksEditViewModel';

    var categories: TranslatedEnumField[] = [{ id: "Official", name: "Official" }, { id: "Commercial", name: "Commercial" }];
    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots", disabled: false };

    QUnit.module("WebLinksEditViewModel");

    test("constructor", () => {

        var target = new WebLinksEditViewModel([webLinkData], categories);

		equal(target.items().length, 1, "webLinks.length");
        equal(target.categories.length, 2, "categories.length");

    });

    test("add new", () => {

        var target = new WebLinksEditViewModel([]);

        target.add();

		equal(target.items().length, 1, "webLinks.length");

    });

    test("remove", () => {

        var target = new WebLinksEditViewModel([webLinkData]);

		target.remove(target.items()[0]);

		equal(target.items().length, 0, "webLinks.length");

    });