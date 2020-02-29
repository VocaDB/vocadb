/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Models/WebLinkCategory.ts" />
/// <reference path="../../Shared/WebLinkMatcher.ts" />

namespace vdb.tests.utils {

    import uti = vdb.utils;

    QUnit.module("WebLinkMatcher");

    QUnit.test("matchWebLink match", () => {

        var result = uti.WebLinkMatcher.matchWebLink("http://www.youtube.com/user/tripshots");

        ok(result, "result");
        equal(result.desc, "YouTube Channel", "desc");
        equal(result.cat, vdb.models.WebLinkCategory.Official, "cat");

    });

	QUnit.test("matchWebLink no match", () => {

        var result = uti.WebLinkMatcher.matchWebLink("http://www.google.com");

        equal(result, null, "result");

    });

}