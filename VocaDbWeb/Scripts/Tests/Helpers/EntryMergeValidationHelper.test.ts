import { EntryMergeValidationHelper } from '@/Helpers/EntryMergeValidationHelper';
import { EntryStatus } from '@/Models/EntryStatus';
import dayjs, { Dayjs } from 'dayjs';

function testValidate(
	expectedLessComplete: boolean,
	expectedNewer: boolean,
	baseStatus: EntryStatus,
	targetStatus: EntryStatus,
	baseDate: Dayjs,
	targetDate: Dayjs,
): void {
	var result = EntryMergeValidationHelper.validate(
		baseStatus,
		targetStatus,
		baseDate.toISOString(),
		targetDate.toISOString(),
	);
	expect(
		result.validationError_targetIsLessComplete,
		'expectedLessComplete',
	).toBe(expectedLessComplete);
	expect(result.validationError_targetIsNewer, 'expectedNewer').toBe(
		expectedNewer,
	);
}

/*function testTargetIsLessComplete(expectedLessComplete: boolean, baseStatus: EntryStatus, targetStatus: EntryStatus, baseDate: moment.Moment, targetDate: moment.Moment) {
		var result = vdb.helpers.EntryMergeValidationHelper.validate(EntryStatus[baseStatus], EntryStatus[targetStatus], baseDate.toISOString(), targetDate.toISOString());
		QUnit.equal(result.validationError_targetIsLessComplete, expectedLessComplete, "expectedLessComplete");
	}

	function testTargetIsNewer(expectedNewer: boolean, baseStatus: EntryStatus, targetStatus: EntryStatus, baseDate: moment.Moment, targetDate: moment.Moment) {
		var result = vdb.helpers.EntryMergeValidationHelper.validate(EntryStatus[baseStatus], EntryStatus[targetStatus], baseDate.toISOString(), targetDate.toISOString());
		QUnit.equal(result.validationError_targetIsNewer, expectedNewer, "expectedNewer");
	}*/

// Always prefer merging from newer to older, except when the older entry is draft while the newer is more complete

test('target is older, no warning', () => {
	testValidate(
		false,
		false,
		EntryStatus.Draft,
		EntryStatus.Draft,
		dayjs(3939),
		dayjs(39),
	);
});

test('target is older and more complete, no warning', () => {
	testValidate(
		false,
		false,
		EntryStatus.Draft,
		EntryStatus.Finished,
		dayjs(3939),
		dayjs(39),
	);
});

test('target is newer but more complete, no warning', () => {
	testValidate(
		false,
		false,
		EntryStatus.Draft,
		EntryStatus.Finished,
		dayjs(39),
		dayjs(3939),
	);
});

test('target is newer, show warning', () => {
	testValidate(
		false,
		true,
		EntryStatus.Draft,
		EntryStatus.Draft,
		dayjs(39),
		dayjs(3939),
	);
});

test('target is older but draft, show warning', () => {
	testValidate(
		true,
		false,
		EntryStatus.Finished,
		EntryStatus.Draft,
		dayjs(3939),
		dayjs(39),
	);
});

test("target is newer and draft, show only 'target is newer' warning", () => {
	testValidate(
		false,
		true,
		EntryStatus.Finished,
		EntryStatus.Draft,
		dayjs(39),
		dayjs(3939),
	);
});
