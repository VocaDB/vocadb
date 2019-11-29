/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

//module vdb.tests.functions {

    import fu = vdb.functions;

    QUnit.module("GlobalFunctions");

    function mergeUrls(base: string, relative: string) {
        return fu.mergeUrls(base, relative);
    }

    test("mergeUrls bothWithSlash", () => {
        
        var result = mergeUrls("/", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls baseWithSlash", () => {

        var result = mergeUrls("/", "Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls relativeWithSlash", () => {

        var result = mergeUrls("", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls neitherWithSlash", () => {

        var result = mergeUrls("", "Song");

        equal(result, "/Song", "result")

    });
//}