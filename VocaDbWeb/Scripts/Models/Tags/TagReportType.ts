enum TagReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default TagReportType;

export const tagReportTypesWithRequiredNotes = [
	TagReportType.InvalidInfo,
	TagReportType.Other,
];
