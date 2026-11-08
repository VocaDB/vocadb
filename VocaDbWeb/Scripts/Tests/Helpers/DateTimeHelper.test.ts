import { DateTimeHelper } from '@/Helpers/DateTimeHelper';

test('convertToLocal preserves the UTC calendar date at local midnight', () => {
	const result = DateTimeHelper.convertToLocal(new Date('2024-02-29T23:59:59Z'));
	expect(result).toEqual(new Date(2024, 1, 29));
});

test('convertToLocal null', () => {
	expect(DateTimeHelper.convertToLocal(null!)).toBeNull();
});

test.each([
	[2016, 9, 3, '2016-10-03T00:00:00.000Z'],
	[2024, 1, 29, '2024-02-29T00:00:00.000Z'],
	[2024, 2, 10, '2024-03-10T00:00:00.000Z'],
	[2024, 10, 3, '2024-11-03T00:00:00.000Z'],
	[2024, 11, 31, '2024-12-31T00:00:00.000Z'],
	[2025, 0, 1, '2025-01-01T00:00:00.000Z'],
	[99, 0, 2, '0099-01-02T00:00:00.000Z'],
] as [number, number, number, string][])('convertToUtc uses midnight for %s/%s/%s', (year, month, day, expected) => {
	const local = new Date(0);
	local.setFullYear(year, month, day);
	local.setHours(18, 17, 16, 15);
	const original = local.getTime();

	expect(DateTimeHelper.convertToUtc(local)!.toISOString()).toBe(expected);
	expect(local.getTime()).toBe(original);
	expect(DateTimeHelper.convertToLocal(DateTimeHelper.convertToUtc(local)!)).toEqual(
		new Date(year < 100 ? 2000 : year, month, day),
	);
});

test('convertToUtc is independent of the current clock and month', () => {
	jest.useFakeTimers().setSystemTime(new Date('2031-01-31T18:17:16.015Z'));
	try {
		expect(DateTimeHelper.convertToUtc(new Date(2024, 1, 29))!.toISOString()).toBe(
			'2024-02-29T00:00:00.000Z',
		);
	} finally {
		jest.useRealTimers();
	}
});

test.each([null, undefined])('convertToUtc returns null for %s', (value) => {
	expect(DateTimeHelper.convertToUtc(value!)).toBeNull();
});
