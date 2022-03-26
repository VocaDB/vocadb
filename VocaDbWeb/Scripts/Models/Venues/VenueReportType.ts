enum VenueReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default VenueReportType;

export const reportTypesWithRequiredNotes = [
	VenueReportType.InvalidInfo,
	VenueReportType.Other,
];
