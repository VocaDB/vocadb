export enum TagReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export const tagReportTypesWithRequiredNotes = [
	TagReportType.InvalidInfo,
	TagReportType.Other,
];
