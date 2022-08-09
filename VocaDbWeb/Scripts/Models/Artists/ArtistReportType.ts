export enum ArtistReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	OwnershipClaim = 'OwnershipClaim',
	Other = 'Other',
}

export const artistReportTypesWithRequiredNotes = [
	ArtistReportType.InvalidInfo,
	ArtistReportType.Other,
];
