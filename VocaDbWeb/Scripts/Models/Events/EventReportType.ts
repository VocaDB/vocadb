export enum EventReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export const eventReportTypesWithRequiredNotes = [
	EventReportType.InvalidInfo,
	EventReportType.Other,
];
