enum EventReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default EventReportType;

export const reportTypesWithRequiredNotes = [
	EventReportType.InvalidInfo,
	EventReportType.Other,
];
