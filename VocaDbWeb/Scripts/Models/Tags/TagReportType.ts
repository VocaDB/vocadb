enum TagReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default TagReportType;

export const reportTypesWithRequiredNotes = [
	TagReportType.InvalidInfo,
	TagReportType.Other,
];
