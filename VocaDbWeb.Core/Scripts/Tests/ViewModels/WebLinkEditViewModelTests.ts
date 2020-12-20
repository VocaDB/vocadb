import WebLinkEditViewModel from '../../ViewModels/WebLinkEditViewModel';

    var webLinkData = { category: "Official", description: "Youtube Channel", id: 0, url: "http://www.youtube.com/user/tripshots", disabled: false };

    QUnit.module("WebLinkEditViewModel");

    test("constructor", () => {

        var target = new WebLinkEditViewModel(webLinkData);

        equal(target.category(), "Official", "category");
        equal(target.description(), "Youtube Channel", "description");
        equal(target.url(), "http://www.youtube.com/user/tripshots", "url");

    });

    test("editing url sets description", () => {

        var target = new WebLinkEditViewModel(null);

        target.url("http://www.nicovideo.jp/mylist/");

        equal(target.category(), "Official", "category");
        equal(target.description(), "NND MyList", "description");

    });