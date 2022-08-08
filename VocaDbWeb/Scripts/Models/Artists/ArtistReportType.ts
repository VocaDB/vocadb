enum ArtistReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	OwnershipClaim = 'OwnershipClaim',
	Other = 'Other',
}

export default ArtistReportType;

export const artistReportTypesWithRequiredNotes = [
	ArtistReportType.InvalidInfo,
	ArtistReportType.Other,
];
