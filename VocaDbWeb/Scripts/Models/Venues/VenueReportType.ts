enum VenueReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default VenueReportType;

export const venueReportTypesWithRequiredNotes = [
	VenueReportType.InvalidInfo,
	VenueReportType.Other,
];
