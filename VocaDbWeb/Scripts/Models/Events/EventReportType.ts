enum EventReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default EventReportType;

export const eventReportTypesWithRequiredNotes = [
	EventReportType.InvalidInfo,
	EventReportType.Other,
];
