enum ArtistReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export default ArtistReportType;

export const reportTypesWithRequiredNotes = [
	ArtistReportType.InvalidInfo,
	ArtistReportType.Other,
];
