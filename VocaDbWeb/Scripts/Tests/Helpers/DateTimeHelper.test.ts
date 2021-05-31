import DateTimeHelper from '@Helpers/DateTimeHelper';

test('convertToLocal', () => {
	var date = new Date(2016, 9, 3);
	date.setMinutes(date.getMinutes() - date.getTimezoneOffset());
	var result = DateTimeHelper.convertToLocal(date);
	var expected = new Date(2016, 9, 3);

	expect(result!.toString()).toBe(expected.toString());
});

test('convertToLocal null', () => {
	var result = DateTimeHelper.convertToLocal(null!);

	expect(result, 'result').toBeNull();
});

test('convertToUtc', () => {
	var date = new Date(2016, 9, 3);
	var result = DateTimeHelper.convertToUtc(date);
	date.setMinutes(date.getMinutes() - date.getTimezoneOffset());

	expect(result!.toString()).toBe(date.toString());
});

test('convertToUtc null', () => {
	var result = DateTimeHelper.convertToUtc(null!);

	expect(result, 'result').toBeNull();
});
