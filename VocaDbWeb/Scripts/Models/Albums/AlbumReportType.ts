export enum AlbumReportType {
	InvalidInfo = 'InvalidInfo',
	Duplicate = 'Duplicate',
	Inappropriate = 'Inappropriate',
	Other = 'Other',
}

export const albumReportTypesWithRequiredNotes = [
	AlbumReportType.InvalidInfo,
	AlbumReportType.Other,
];
