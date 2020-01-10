/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

//module vdb.tests.functions {

    QUnit.module("GlobalFunctions");

    test("mergeUrls bothWithSlash", () => {
        
        var result = vdb.functions.mergeUrls("/", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls baseWithSlash", () => {

		var result = vdb.functions.mergeUrls("/", "Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls relativeWithSlash", () => {

		var result = vdb.functions.mergeUrls("", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls neitherWithSlash", () => {

		var result = vdb.functions.mergeUrls("", "Song");

        equal(result, "/Song", "result")

    });
//}