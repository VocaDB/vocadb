export enum VenueReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export const venueReportTypesWithRequiredNotes = [
	VenueReportType.InvalidInfo,
	VenueReportType.Other,
];
