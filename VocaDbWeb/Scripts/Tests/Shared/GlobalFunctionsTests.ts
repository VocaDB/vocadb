/// <reference path="../../typings/qunit/qunit.d.ts" />
/// <reference path="../../Shared/GlobalFunctions.ts" />

import functions from "../../Shared/GlobalFunctions";

//module vdb.tests.functions {

    QUnit.module("GlobalFunctions");

    test("mergeUrls bothWithSlash", () => {

		var result = functions.mergeUrls("/", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls baseWithSlash", () => {

		var result = functions.mergeUrls("/", "Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls relativeWithSlash", () => {

		var result = functions.mergeUrls("", "/Song");

        equal(result, "/Song", "result")

    });

    test("mergeUrls neitherWithSlash", () => {

		var result = functions.mergeUrls("", "Song");

        equal(result, "/Song", "result")

    });
//}