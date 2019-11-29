
//module vdb.tests.helpers {

	QUnit.module("DateTimeHelper");

	QUnit.test("convertToLocal", () => {

		var date = new Date(2016, 9, 3);
		date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
		var result = vdb.helpers.DateTimeHelper.convertToLocal(date);
		var expected = new Date(2016, 9, 3);

		QUnit.equal(result.toString(), expected.toString());

	});

	QUnit.test("convertToLocal null", () => {

		var result = vdb.helpers.DateTimeHelper.convertToLocal(null);

		QUnit.equal(result, null, "result");

	});

	QUnit.test("convertToUtc", () => {

		var date = new Date(2016, 9, 3);
		var result = vdb.helpers.DateTimeHelper.convertToUtc(date);
		date.setMinutes(date.getMinutes() - date.getTimezoneOffset());

		QUnit.equal(result.toString(), date.toString());

	});

	QUnit.test("convertToUtc null", () => {

		var result = vdb.helpers.DateTimeHelper.convertToUtc(null);

		QUnit.equal(result, null, "result");

	});

//}