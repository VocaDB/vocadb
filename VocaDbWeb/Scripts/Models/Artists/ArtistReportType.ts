export enum ArtistReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	OwnershipClaim = 'OwnershipClaim',
	Other = 'Other',
	InvalidTag = 'InvalidTag',
}

export const artistReportTypesWithRequiredNotes = [
	ArtistReportType.InvalidInfo,
	ArtistReportType.Other,
	ArtistReportType.InvalidTag,
];
