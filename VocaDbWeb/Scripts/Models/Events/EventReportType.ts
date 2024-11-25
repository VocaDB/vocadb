export enum EventReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
	InvalidTag = 'InvalidTag',
}

export const eventReportTypesWithRequiredNotes = [
	EventReportType.InvalidInfo,
	EventReportType.Other,
	EventReportType.InvalidTag,
];
